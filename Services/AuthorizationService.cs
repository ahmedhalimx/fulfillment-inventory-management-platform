using Microsoft.EntityFrameworkCore;
using FulfillmentInventoryPlatform.API.Data;
using FulfillmentInventoryPlatform.API.Enums;

namespace FulfillmentInventoryPlatform.API.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly AppDbContext _context;

    public AuthorizationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanUserAccessWarehouseAsync(int userId, UserRole role, int warehouseId)
    {
        if (role == UserRole.Admin || role == UserRole.Manager)
        {
            return true;
        }

        if (role == UserRole.Operator)
        {
            return await _context.UserWarehouses
                .AnyAsync(uw => uw.UserId == userId && uw.WarehouseId == warehouseId);
        }

        return false;
    }
}
