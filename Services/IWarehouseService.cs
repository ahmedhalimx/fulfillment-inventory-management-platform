using FulfillmentInventoryPlatform.API.Dtos.Warehouse;

namespace FulfillmentInventoryPlatform.API.Services;

public interface IWarehouseService
{
    Task<List<WarehouseResponse>> GetAllAsync();
    Task<WarehouseResponse> GetByIdAsync(int id);
    Task<WarehouseResponse> CreateAsync(WarehouseCreateRequest request);
    Task<WarehouseResponse> UpdateAsync(int id, WarehouseUpdateRequest request);
    Task SoftDeleteAsync(int id);
}
