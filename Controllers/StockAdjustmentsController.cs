using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FulfillmentInventoryPlatform.API.Dtos.Common;
using FulfillmentInventoryPlatform.API.Dtos.Stock;
using FulfillmentInventoryPlatform.API.Enums;
using FulfillmentInventoryPlatform.API.Services;

namespace FulfillmentInventoryPlatform.API.Controllers;

[ApiController]
[Route("api/stock-adjustments")]
[Authorize]
public class StockAdjustmentsController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockAdjustmentsController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<StockAdjustmentResponse>>> GetAdjustments(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] int? productId = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int? performedBy = null)
    {
        var (userId, role) = GetUserContext();
        var result = await _stockService.GetStockAdjustmentsPagedAsync(
            page, size, productId, warehouseId, from, to, performedBy, userId, role);

        return Ok(result);
    }

    private (int UserId, UserRole Role) GetUserContext()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);

        if (!int.TryParse(userIdClaim, out int userId))
            throw new UnauthorizedAccessException("Invalid user identity claim.");

        Enum.TryParse<UserRole>(roleClaim, true, out var role);
        return (userId, role);
    }
}
