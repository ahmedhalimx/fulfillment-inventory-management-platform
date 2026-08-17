namespace FulfillmentInventoryPlatform.API.Dtos.Report;

public class ProductStockSummary
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
}

public class WarehouseStockSummary
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
}

public class StockSummaryResponse
{
    public int TotalStockAllWarehouses { get; set; }
    public int ZeroStockProductCount { get; set; }
    public int RecentAdjustmentsCount { get; set; }
    public List<WarehouseStockSummary> StockPerWarehouse { get; set; } = new();
    public List<ProductStockSummary> StockPerProduct { get; set; } = new();
}
