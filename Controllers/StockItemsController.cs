using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FulfillmentInventoryPlatform.API.Dtos.Common;
using FulfillmentInventoryPlatform.API.Dtos.Stock;
using FulfillmentInventoryPlatform.API.Services;

namespace FulfillmentInventoryPlatform.API.Controllers;

[ApiController]
[Route("api/stock-items")]
[Authorize]
public class StockItemsController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockItemsController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<StockItemResponse>>> GetStockItems(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] int? productId = null,
        [FromQuery] int? warehouseId = null)
    {
        var (userId, role) = GetUserContext();
        var result = await _stockService.GetStockItemsPagedAsync(page, size, productId, warehouseId, userId, role);
        return Ok(result);
    }

    [HttpPost("adjust")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<StockAdjustmentResponse>> AdjustStock([FromBody] StockAdjustmentRequest request)
    {
        var (userId, role) = GetUserContext();
        var result = await _stockService.AdjustStockAsync(request, userId, role);
        return Ok(result);
    }

    private (int UserId, string Role) GetUserContext()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);

        if (!int.TryParse(userIdClaim, out int userId))
            throw new UnauthorizedAccessException("Invalid user identity claim.");

        return (userId, roleClaim ?? "Operator");
    }
}
