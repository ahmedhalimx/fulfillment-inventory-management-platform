using System.Text.Json.Serialization;

namespace FulfillmentInventoryPlatform.API.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AdjustmentType
{
    Receive,
    Ship,
    Damaged,
    Correction,
    Other
}
