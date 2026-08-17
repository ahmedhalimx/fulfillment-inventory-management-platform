using Microsoft.EntityFrameworkCore;
using FulfillmentInventoryPlatform.API.Data;
using FulfillmentInventoryPlatform.API.Dtos.Common;
using FulfillmentInventoryPlatform.API.Dtos.Stock;
using FulfillmentInventoryPlatform.API.Models;

namespace FulfillmentInventoryPlatform.API.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public StockService(AppDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<PagedResult<StockItemResponse>> GetStockItemsPagedAsync(
        int page,
        int size,
        int? productId,
        int? warehouseId,
        int currentUserId,
        string currentUserRole)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var query = _context.StockItems
            .Include(si => si.Product)
            .Include(si => si.Warehouse)
            .AsQueryable();

        // RBAC filtering
        if (string.Equals(currentUserRole, "Operator", StringComparison.OrdinalIgnoreCase))
        {
            var assignedWarehouseIds = await _context.UserWarehouses
                .Where(uw => uw.UserId == currentUserId)
                .Select(uw => uw.WarehouseId)
                .ToListAsync();

            query = query.Where(si => assignedWarehouseIds.Contains(si.WarehouseId));
        }

        if (productId.HasValue)
        {
            query = query.Where(si => si.ProductId == productId.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(si => si.WarehouseId == warehouseId.Value);
        }

        int totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(si => si.Warehouse.Name)
            .ThenBy(si => si.Product.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(si => new StockItemResponse
            {
                Id = si.Id,
                ProductId = si.ProductId,
                ProductName = si.Product.Name,
                ProductSKU = si.Product.SKU,
                ProductIsDeleted = si.Product.IsDeleted,
                WarehouseId = si.WarehouseId,
                WarehouseName = si.Warehouse.Name,
                Quantity = si.Quantity,
                LastUpdatedAt = si.LastUpdatedAt
            })
            .ToListAsync();

        return new PagedResult<StockItemResponse>(items, page, size, totalCount);
    }

    public async Task<StockAdjustmentResponse> AdjustStockAsync(
        StockAdjustmentRequest request,
        int currentUserId,
        string currentUserRole)
    {
        // 1. Authorization check (Only Admin or assigned Operator can adjust stock)
        if (string.Equals(currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Managers are not authorized to perform stock adjustments.");
        }

        bool canAccess = await _authorizationService.CanUserAccessWarehouseAsync(currentUserId, currentUserRole, request.WarehouseId);
        if (!canAccess)
        {
            throw new UnauthorizedAccessException($"User is not authorized to adjust stock in warehouse ID {request.WarehouseId}.");
        }

        // 2. Validate input parameters
        if (!request.Delta.HasValue && !request.NewQuantity.HasValue)
        {
            throw new ArgumentException("Either 'delta' or 'newQuantity' must be provided.");
        }

        // 3. Verify product and warehouse exist and check soft-delete state
        var product = await _context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == request.ProductId);

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.ProductId} does not exist.");
        }

        if (product.IsDeleted)
        {
            throw new InvalidOperationException($"Cannot adjust stock for soft-deleted product '{product.Name}'.");
        }

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId);

        if (warehouse == null)
        {
            throw new KeyNotFoundException($"Active warehouse with ID {request.WarehouseId} not found.");
        }

        // Use DB Transaction to ensure atomic update of stock item and creation of stock adjustment log
        var executionStrategy = _context.Database.CreateExecutionStrategy();

        StockAdjustment adjustment = null!;

        await executionStrategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var stockItem = await _context.StockItems
                .FirstOrDefaultAsync(si => si.ProductId == request.ProductId && si.WarehouseId == request.WarehouseId);

            if (stockItem == null)
            {
                stockItem = new StockItem
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    Quantity = 0,
                    LastUpdatedAt = DateTime.UtcNow
                };
                _context.StockItems.Add(stockItem);
                await _context.SaveChangesAsync();
            }

            int prevQty = stockItem.Quantity;
            int newQty;
            int delta;

            if (request.NewQuantity.HasValue)
            {
                newQty = request.NewQuantity.Value;
                delta = newQty - prevQty;
            }
            else
            {
                delta = request.Delta!.Value;
                newQty = prevQty + delta;
            }

            if (newQty < 0)
            {
                throw new InvalidOperationException($"Stock quantity cannot drop below 0. (Current: {prevQty}, Requested change: {delta}, Resulting: {newQty})");
            }

            stockItem.Quantity = newQty;
            stockItem.LastUpdatedAt = DateTime.UtcNow;

            adjustment = new StockAdjustment
            {
                StockItemId = stockItem.Id,
                PreviousQuantity = prevQty,
                NewQuantity = newQty,
                QuantityDelta = delta,
                AdjustmentType = string.IsNullOrWhiteSpace(request.AdjustmentType) ? "Other" : request.AdjustmentType,
                Note = request.Note,
                PerformedByUserId = currentUserId,
                PerformedAt = DateTime.UtcNow
            };

            _context.StockAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        });

        var user = await _context.Users.FindAsync(currentUserId);

        return new StockAdjustmentResponse
        {
            Id = adjustment.Id,
            StockItemId = adjustment.StockItemId,
            ProductId = product.Id,
            ProductName = product.Name,
            ProductIsDeleted = product.IsDeleted,
            WarehouseId = warehouse.Id,
            WarehouseName = warehouse.Name,
            PreviousQuantity = adjustment.PreviousQuantity,
            NewQuantity = adjustment.NewQuantity,
            QuantityDelta = adjustment.QuantityDelta,
            AdjustmentType = adjustment.AdjustmentType,
            Note = adjustment.Note,
            PerformedByUserId = currentUserId,
            PerformedByUsername = user?.Username ?? string.Empty,
            PerformedAt = adjustment.PerformedAt
        };
    }

    public async Task<PagedResult<StockAdjustmentResponse>> GetStockAdjustmentsPagedAsync(
        int page,
        int size,
        int? productId,
        int? warehouseId,
        DateTime? from,
        DateTime? to,
        int? performedByUserId,
        int currentUserId,
        string currentUserRole)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var query = _context.StockAdjustments
            .Include(sa => sa.StockItem)
                .ThenInclude(si => si.Product)
            .Include(sa => sa.StockItem)
                .ThenInclude(si => si.Warehouse)
            .Include(sa => sa.PerformedBy)
            .IgnoreQueryFilters() // Ignore query filters to keep soft-deleted products/warehouses legible in history
            .AsQueryable();

        // RBAC filtering
        if (string.Equals(currentUserRole, "Operator", StringComparison.OrdinalIgnoreCase))
        {
            var assignedWarehouseIds = await _context.UserWarehouses
                .Where(uw => uw.UserId == currentUserId)
                .Select(uw => uw.WarehouseId)
                .ToListAsync();

            query = query.Where(sa => sa.PerformedByUserId == currentUserId || assignedWarehouseIds.Contains(sa.StockItem.WarehouseId));
        }

        if (productId.HasValue)
        {
            query = query.Where(sa => sa.StockItem.ProductId == productId.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(sa => sa.StockItem.WarehouseId == warehouseId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(sa => sa.PerformedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(sa => sa.PerformedAt <= to.Value);
        }

        if (performedByUserId.HasValue)
        {
            query = query.Where(sa => sa.PerformedByUserId == performedByUserId.Value);
        }

        int totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(sa => sa.PerformedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(sa => new StockAdjustmentResponse
            {
                Id = sa.Id,
                StockItemId = sa.StockItemId,
                ProductId = sa.StockItem.ProductId,
                ProductName = sa.StockItem.Product.Name,
                ProductIsDeleted = sa.StockItem.Product.IsDeleted,
                WarehouseId = sa.StockItem.WarehouseId,
                WarehouseName = sa.StockItem.Warehouse.Name,
                PreviousQuantity = sa.PreviousQuantity,
                NewQuantity = sa.NewQuantity,
                QuantityDelta = sa.QuantityDelta,
                AdjustmentType = sa.AdjustmentType,
                Note = sa.Note,
                PerformedByUserId = sa.PerformedByUserId,
                PerformedByUsername = sa.PerformedBy.Username,
                PerformedAt = sa.PerformedAt
            })
            .ToListAsync();

        return new PagedResult<StockAdjustmentResponse>(items, page, size, totalCount);
    }
}
