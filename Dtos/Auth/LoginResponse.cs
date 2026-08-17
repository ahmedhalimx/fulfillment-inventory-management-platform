using FulfillmentInventoryPlatform.API.Dtos.User;

namespace FulfillmentInventoryPlatform.API.Dtos.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
