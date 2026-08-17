using Microsoft.EntityFrameworkCore;
using FulfillmentInventoryPlatform.API.Data;

namespace FulfillmentInventoryPlatform.API.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly AppDbContext _context;

    public AuthorizationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanUserAccessWarehouseAsync(int userId, string role, int warehouseId)
    {
        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, "Manager", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(role, "Operator", StringComparison.OrdinalIgnoreCase))
        {
            return await _context.UserWarehouses
                .AnyAsync(uw => uw.UserId == userId && uw.WarehouseId == warehouseId);
        }

        return false;
    }
}
