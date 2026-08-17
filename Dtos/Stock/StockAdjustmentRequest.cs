using System.ComponentModel.DataAnnotations;
using FulfillmentInventoryPlatform.API.Enums;

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
    public AdjustmentType AdjustmentType { get; set; } = AdjustmentType.Other;

    public string? Note { get; set; }
}
