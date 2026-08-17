using FulfillmentInventoryPlatform.API.Dtos.Report;

namespace FulfillmentInventoryPlatform.API.Services;

public interface IReportService
{
    Task<StockSummaryResponse> GetStockSummaryAsync();
}
