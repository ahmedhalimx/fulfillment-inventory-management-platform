using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.API.Dtos.Product;

public class ProductCreateRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }
}
