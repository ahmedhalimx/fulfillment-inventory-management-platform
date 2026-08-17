namespace FulfillmentInventoryPlatform.Dtos.Report;


public class ProductStockSummary
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public int TotalQuantityAcrossWarehouses { get; set; }

    public int WarehouseCount { get; set; }
}
