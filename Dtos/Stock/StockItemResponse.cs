namespace FulfillmentInventoryPlatform.API.Dtos.Stock;

public class StockItemResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public bool ProductIsDeleted { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
