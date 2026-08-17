namespace FulfillmentInventoryPlatform.Dtos.Product;


public class ProductCreateRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public int CategoryId { get; set; }
}
