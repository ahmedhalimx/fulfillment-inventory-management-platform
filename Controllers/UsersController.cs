using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FulfillmentInventoryPlatform.API.Dtos.User;
using FulfillmentInventoryPlatform.API.Services;

namespace FulfillmentInventoryPlatform.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("{userId}/role")]
    public async Task<ActionResult<UserDto>> UpdateRole(int userId, [FromBody] UserRoleUpdateRequest request)
    {
        var user = await _userService.UpdateRoleAsync(userId, request.Role);
        return Ok(user);
    }

    [HttpPut("{userId}/warehouses")]
    public async Task<ActionResult<UserDto>> AssignWarehouses(int userId, [FromBody] List<int> warehouseIds)
    {
        var user = await _userService.AssignWarehousesAsync(userId, warehouseIds);
        return Ok(user);
    }

    [HttpDelete("{userId}/warehouses/{warehouseId}")]
    public async Task<ActionResult<UserDto>> RemoveWarehouseAssignment(int userId, int warehouseId)
    {
        var user = await _userService.RemoveWarehouseAssignmentAsync(userId, warehouseId);
        return Ok(user);
    }
}
