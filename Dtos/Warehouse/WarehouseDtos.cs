using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.API.Dtos.Warehouse;

public class WarehouseCreateRequest
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;
}

public class WarehouseUpdateRequest
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;
}

public class WarehouseResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
