using System.ComponentModel.DataAnnotations;

namespace FulfillmentInventoryPlatform.API.Dtos.User;

public class UserWarehouseAssignRequest
{
    [Required]
    public List<int> WarehouseIds { get; set; } = new();
}
