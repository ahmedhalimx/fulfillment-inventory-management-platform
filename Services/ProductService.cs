using Microsoft.EntityFrameworkCore;
using FulfillmentInventoryPlatform.API.Data;
using FulfillmentInventoryPlatform.API.Dtos.Common;
using FulfillmentInventoryPlatform.API.Dtos.Product;
using FulfillmentInventoryPlatform.API.Models;

namespace FulfillmentInventoryPlatform.API.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductResponse>> GetPagedAsync(
        int page,
        int size,
        string? sort,
        string? order,
        int? categoryId,
        string? search)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchLower) || p.SKU.ToLower().Contains(searchLower));
        }

        bool isDescending = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);

        query = (sort?.ToLower()) switch
        {
            "sku" => isDescending ? query.OrderByDescending(p => p.SKU) : query.OrderBy(p => p.SKU),
            "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
        };

        int totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => MapToResponse(p))
            .ToListAsync();

        return new PagedResult<ProductResponse>(items, page, size, totalCount);
    }

    public async Task<ProductResponse> GetByIdAsync(int id)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        return MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(ProductCreateRequest request)
    {
        if (await _context.Products.AnyAsync(p => p.SKU == request.SKU))
            throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists.");

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {request.CategoryId} does not exist.");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            SKU = request.SKU,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        product.Category = category;
        return MapToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(int id, ProductUpdateRequest request)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        if (await _context.Products.AnyAsync(p => p.SKU == request.SKU && p.Id != id))
            throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists.");

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {request.CategoryId} does not exist.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.SKU = request.SKU;
        product.CategoryId = request.CategoryId;
        product.Category = category;

        await _context.SaveChangesAsync();
        return MapToResponse(product);
    }

    public async Task SoftDeleteAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        product.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    private static ProductResponse MapToResponse(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        SKU = p.SKU,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty,
        IsDeleted = p.IsDeleted,
        CreatedAt = p.CreatedAt
    };
}
