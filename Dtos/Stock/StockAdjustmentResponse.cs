namespace FulfillmentInventoryPlatform.Dtos.Stock;

public class StockAdjustmentResponse
{
    public int Id { get; set; }

    public int StockItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public int PreviousQuantity { get; set; }

    public int NewQuantity { get; set; }

    public int QuantityDelta { get; set; }

    public string AdjustmentType { get; set; } = string.Empty;

    public string? Note { get; set; }

    public string PerformedBy { get; set; } = string.Empty;

    public DateTime PerformedAt { get; set; }
}
