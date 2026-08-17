namespace FulfillmentInventoryPlatform.Dtos.Report;


public class StockSummaryResponse
{
    public List<WarehouseStockSummary> WarehouseSummaries { get; set; } = new();

    public List<ProductStockSummary> ProductSummaries { get; set; } = new();

    public int ZeroStockProductCount { get; set; }

    public int RecentAdjustmentsCount { get; set; }
}
