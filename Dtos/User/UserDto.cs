using FulfillmentInventoryPlatform.API.Enums;

namespace FulfillmentInventoryPlatform.API.Dtos.User;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<int> AssignedWarehouseIds { get; set; } = new();
}
