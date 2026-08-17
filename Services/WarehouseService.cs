using Microsoft.EntityFrameworkCore;
using FulfillmentInventoryPlatform.API.Data;
using FulfillmentInventoryPlatform.API.Dtos.Warehouse;
using FulfillmentInventoryPlatform.API.Models;

namespace FulfillmentInventoryPlatform.API.Services;

public class WarehouseService : IWarehouseService
{
    private readonly AppDbContext _context;

    public WarehouseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WarehouseResponse>> GetAllAsync()
    {
        return await _context.Warehouses
            .Select(w => MapToResponse(w))
            .ToListAsync();
    }

    public async Task<WarehouseResponse> GetByIdAsync(int id)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id);
        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID {id} not found.");

        return MapToResponse(warehouse);
    }

    public async Task<WarehouseResponse> CreateAsync(WarehouseCreateRequest request)
    {
        var warehouse = new Warehouse
        {
            Name = request.Name,
            Location = request.Location,
            CreatedAt = DateTime.UtcNow
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return MapToResponse(warehouse);
    }

    public async Task<WarehouseResponse> UpdateAsync(int id, WarehouseUpdateRequest request)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id);
        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID {id} not found.");

        warehouse.Name = request.Name;
        warehouse.Location = request.Location;

        await _context.SaveChangesAsync();
        return MapToResponse(warehouse);
    }

    public async Task SoftDeleteAsync(int id)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id);
        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID {id} not found.");

        warehouse.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    private static WarehouseResponse MapToResponse(Warehouse w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        Location = w.Location,
        IsDeleted = w.IsDeleted,
        CreatedAt = w.CreatedAt
    };
}
