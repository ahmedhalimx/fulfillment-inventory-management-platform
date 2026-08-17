namespace FulfillmentInventoryPlatform.API.Services;

public interface IAuthorizationService
{
    Task<bool> CanUserAccessWarehouseAsync(int userId, string role, int warehouseId);
}
