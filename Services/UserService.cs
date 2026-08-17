using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using FulfillmentInventoryPlatform.API.Data;
using FulfillmentInventoryPlatform.API.Dtos.Auth;
using FulfillmentInventoryPlatform.API.Dtos.User;
using FulfillmentInventoryPlatform.API.Enums;
using FulfillmentInventoryPlatform.API.Models;

namespace FulfillmentInventoryPlatform.API.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            throw new InvalidOperationException("Username is already taken.");

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserWarehouses)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        return MapToDto(user);
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.UserWarehouses)
            .ToListAsync();

        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> UpdateRoleAsync(int userId, UserRole newRole)
    {
        var user = await _context.Users
            .Include(u => u.UserWarehouses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        user.Role = newRole;
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto> AssignWarehousesAsync(int userId, List<int> warehouseIds)
    {
        var user = await _context.Users
            .Include(u => u.UserWarehouses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        var existingWarehouses = await _context.Warehouses
            .IgnoreQueryFilters()
            .Where(w => warehouseIds.Contains(w.Id) && !w.IsDeleted)
            .Select(w => w.Id)
            .ToListAsync();

        if (existingWarehouses.Count != warehouseIds.Distinct().Count())
            throw new InvalidOperationException("One or more warehouse IDs are invalid or deleted.");

        _context.UserWarehouses.RemoveRange(user.UserWarehouses);
        foreach (var wId in warehouseIds.Distinct())
        {
            user.UserWarehouses.Add(new UserWarehouse { UserId = userId, WarehouseId = wId });
        }

        await _context.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserDto> RemoveWarehouseAssignmentAsync(int userId, int warehouseId)
    {
        var user = await _context.Users
            .Include(u => u.UserWarehouses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        var assignment = user.UserWarehouses.FirstOrDefault(uw => uw.WarehouseId == warehouseId);
        if (assignment != null)
        {
            _context.UserWarehouses.Remove(assignment);
            await _context.SaveChangesAsync();
        }

        return MapToDto(user);
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        CreatedAt = user.CreatedAt,
        AssignedWarehouseIds = user.UserWarehouses.Select(uw => uw.WarehouseId).ToList()
    };
}
