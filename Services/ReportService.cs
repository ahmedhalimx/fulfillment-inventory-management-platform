using Microsoft.EntityFrameworkCore;
using FulfillmentInventoryPlatform.API.Data;
using FulfillmentInventoryPlatform.API.Dtos.Report;

namespace FulfillmentInventoryPlatform.API.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StockSummaryResponse> GetStockSummaryAsync()
    {
        var stockPerWarehouse = await _context.Warehouses
            .Select(w => new WarehouseStockSummary
            {
                WarehouseId = w.Id,
                WarehouseName = w.Name,
                TotalQuantity = w.StockItems.Sum(si => si.Quantity)
            })
            .ToListAsync();

        var stockPerProduct = await _context.Products
            .Select(p => new ProductStockSummary
            {
                ProductId = p.Id,
                ProductName = p.Name,
                SKU = p.SKU,
                TotalQuantity = p.StockItems.Sum(si => si.Quantity)
            })
            .ToListAsync();

        int totalStockAll = stockPerWarehouse.Sum(w => w.TotalQuantity);
        int zeroStockProductCount = stockPerProduct.Count(p => p.TotalQuantity == 0);

        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        int recentAdjustmentsCount = await _context.StockAdjustments
            .CountAsync(sa => sa.PerformedAt >= thirtyDaysAgo);

        return new StockSummaryResponse
        {
            TotalStockAllWarehouses = totalStockAll,
            ZeroStockProductCount = zeroStockProductCount,
            RecentAdjustmentsCount = recentAdjustmentsCount,
            StockPerWarehouse = stockPerWarehouse,
            StockPerProduct = stockPerProduct
        };
    }
}
