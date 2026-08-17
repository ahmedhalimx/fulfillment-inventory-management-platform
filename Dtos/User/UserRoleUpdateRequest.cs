using System.ComponentModel.DataAnnotations;
using FulfillmentInventoryPlatform.API.Enums;

namespace FulfillmentInventoryPlatform.API.Dtos.User;

public class UserRoleUpdateRequest
{
    [Required]
    public UserRole Role { get; set; }
}
