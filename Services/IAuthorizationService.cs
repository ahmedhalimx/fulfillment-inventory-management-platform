using FulfillmentInventoryPlatform.API.Enums;

namespace FulfillmentInventoryPlatform.API.Services;

public interface IAuthorizationService
{
    Task<bool> CanUserAccessWarehouseAsync(int userId, UserRole role, int warehouseId);
}
