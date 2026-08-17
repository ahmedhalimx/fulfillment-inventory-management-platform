using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.API.Dtos.User;

public class UserRoleUpdateRequest
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
