namespace FulfillmentInventoryPlatform.Dtos.Report;


public class WarehouseStockSummary
{
    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    public int TotalQuantity { get; set; }
}
