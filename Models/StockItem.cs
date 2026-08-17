using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.API.Models;

public class StockItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int Quantity { get; set; }

    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();
}
