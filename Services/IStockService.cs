using FulfillmentInventoryPlatform.API.Dtos.Common;
using FulfillmentInventoryPlatform.API.Dtos.Stock;

namespace FulfillmentInventoryPlatform.API.Services;

public interface IStockService
{
    Task<PagedResult<StockItemResponse>> GetStockItemsPagedAsync(
        int page,
        int size,
        int? productId,
        int? warehouseId,
        int currentUserId,
        string currentUserRole);

    Task<StockAdjustmentResponse> AdjustStockAsync(
        StockAdjustmentRequest request,
        int currentUserId,
        string currentUserRole);

    Task<PagedResult<StockAdjustmentResponse>> GetStockAdjustmentsPagedAsync(
        int page,
        int size,
        int? productId,
        int? warehouseId,
        DateTime? from,
        DateTime? to,
        int? performedByUserId,
        int currentUserId,
        string currentUserRole);
}
