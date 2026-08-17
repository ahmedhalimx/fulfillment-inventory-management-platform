using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.Models;


public class Warehouse
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public ICollection<UserWarehouse> UserWarehouses { get; set; } = new List<UserWarehouse>();

    public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
