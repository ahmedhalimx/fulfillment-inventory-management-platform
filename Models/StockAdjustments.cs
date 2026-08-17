using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.Models;


public class StockAdjustment
{
    public int Id { get; set; }

    public int StockItemId { get; set; }

    public int PreviousQuantity { get; set; }

    public int NewQuantity { get; set; }

    public int QuantityDelta { get; set; }

    public string AdjustmentType { get; set; } = "Other";

    public string? Note { get; set; }

    public int PerformedByUserId { get; set; }

    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;


    public StockItem StockItem { get; set; } = null!;

    public User PerformedBy { get; set; } = null!;
}
