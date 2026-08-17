using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.API.Dtos.Stock;

public class StockAdjustmentRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    public int? Delta { get; set; }

    public int? NewQuantity { get; set; }

    [Required]
    public string AdjustmentType { get; set; } = "Other"; // Receive, Ship, Damaged, Correction, Other

    public string? Note { get; set; }
}
