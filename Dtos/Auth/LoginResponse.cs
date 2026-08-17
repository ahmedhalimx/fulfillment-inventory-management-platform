namespace FulfillmentInventoryPlatform.Dtos.Auth;


public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
