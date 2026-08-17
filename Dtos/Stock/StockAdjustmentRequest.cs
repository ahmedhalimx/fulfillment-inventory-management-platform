namespace FulfillmentInventoryPlatform.Dtos.Stock;


public class StockAdjustmentRequest
{
    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int? Delta { get; set; }

    public int? NewQuantity { get; set; }

    public string AdjustmentType { get; set; } = "Other";

    public string? Note { get; set; }
}
