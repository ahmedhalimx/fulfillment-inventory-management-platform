using System.ComponentModel.DataAnnotations;
using FulfillmentInventoryPlatform.API.Enums;

namespace FulfillmentInventoryPlatform.API.Dtos.Auth;

public class RegisterRequest
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Operator;
}
