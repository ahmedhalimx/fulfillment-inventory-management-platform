using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FulfillmentInventoryPlatform.API.Dtos.Auth;
using FulfillmentInventoryPlatform.API.Dtos.User;
using FulfillmentInventoryPlatform.API.Services;

namespace FulfillmentInventoryPlatform.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest request)
    {
        var response = await _userService.RegisterAsync(request);
        return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
    }
}
