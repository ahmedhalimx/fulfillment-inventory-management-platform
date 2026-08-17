using System.ComponentModel.DataAnnotations;
using FulfillmentInventoryPlatform.API.Enums;

namespace FulfillmentInventoryPlatform.API.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Operator;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserWarehouse> UserWarehouses { get; set; } = new List<UserWarehouse>();
    public ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();
}
