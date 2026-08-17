using FulfillmentInventoryPlatform.API.Dtos.Auth;
using FulfillmentInventoryPlatform.API.Dtos.User;
using FulfillmentInventoryPlatform.API.Enums;

namespace FulfillmentInventoryPlatform.API.Services;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterRequest request);
    Task<UserDto> GetByIdAsync(int id);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> UpdateRoleAsync(int userId, UserRole newRole);
    Task<UserDto> AssignWarehousesAsync(int userId, List<int> warehouseIds);
    Task<UserDto> RemoveWarehouseAssignmentAsync(int userId, int warehouseId);
}
