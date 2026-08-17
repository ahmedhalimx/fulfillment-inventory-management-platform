using System.Text.Json.Serialization;

namespace FulfillmentInventoryPlatform.API.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    Admin,
    Manager,
    Operator
}
