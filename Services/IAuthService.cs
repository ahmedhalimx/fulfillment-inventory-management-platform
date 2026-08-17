using FulfillmentInventoryPlatform.API.Dtos.Auth;
using FulfillmentInventoryPlatform.API.Dtos.User;

namespace FulfillmentInventoryPlatform.API.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
