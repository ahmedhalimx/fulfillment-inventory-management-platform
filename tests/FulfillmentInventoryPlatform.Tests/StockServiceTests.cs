using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using FulfillmentInventoryPlatform.API.Data;
using FulfillmentInventoryPlatform.API.Dtos.Stock;
using FulfillmentInventoryPlatform.API.Enums;
using FulfillmentInventoryPlatform.API.Models;
using FulfillmentInventoryPlatform.API.Services;
using Xunit;

namespace FulfillmentInventoryPlatform.Tests;

public class StockServiceTests
{
    private AppDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new AppDbContext(options);
        return context;
    }

    [Fact]
    public async Task AdjustStockAsync_PositiveDelta_IncreasesQuantityAndLogsAdjustment()
    {
        // Arrange
        var db = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var authService = new AuthorizationService(db);
        var stockService = new StockService(db, authService);

        var product = new Product { Id = 1, Name = "Laptop", SKU = "LP-001" };
        var warehouse = new Warehouse { Id = 1, Name = "Main Warehouse" };
        var user = new User { Id = 1, Username = "admin", Role = UserRole.Admin };

        db.Products.Add(product);
        db.Warehouses.Add(warehouse);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var request = new StockAdjustmentRequest
        {
            ProductId = 1,
            WarehouseId = 1,
            Delta = 10,
            AdjustmentType = AdjustmentType.Receive,
            Note = "Initial stock intake"
        };

        // Act
        var result = await stockService.AdjustStockAsync(request, currentUserId: 1, currentUserRole: UserRole.Admin);

        // Assert
        Assert.Equal(10, result.NewQuantity);
        Assert.Equal(10, result.QuantityDelta);
        Assert.Equal("Laptop", result.ProductName);

        var stockItem = await db.StockItems.FirstOrDefaultAsync(si => si.ProductId == 1 && si.WarehouseId == 1);
        Assert.NotNull(stockItem);
        Assert.Equal(10, stockItem.Quantity);

        var adjustment = await db.StockAdjustments.FirstOrDefaultAsync(sa => sa.StockItemId == stockItem.Id);
        Assert.NotNull(adjustment);
        Assert.Equal(0, adjustment.PreviousQuantity);
        Assert.Equal(10, adjustment.NewQuantity);
        Assert.Equal(AdjustmentType.Receive, adjustment.AdjustmentType);
    }

    [Fact]
    public async Task AdjustStockAsync_ResultingQuantityNegative_ThrowsInvalidOperationException()
    {
        // Arrange
        var db = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var authService = new AuthorizationService(db);
        var stockService = new StockService(db, authService);

        var product = new Product { Id = 1, Name = "Phone", SKU = "PH-001" };
        var warehouse = new Warehouse { Id = 1, Name = "Main Warehouse" };
        db.Products.Add(product);
        db.Warehouses.Add(warehouse);
        db.StockItems.Add(new StockItem { ProductId = 1, WarehouseId = 1, Quantity = 5 });
        await db.SaveChangesAsync();

        var request = new StockAdjustmentRequest
        {
            ProductId = 1,
            WarehouseId = 1,
            Delta = -10,
            AdjustmentType = AdjustmentType.Ship
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stockService.AdjustStockAsync(request, currentUserId: 1, currentUserRole: UserRole.Admin));

        Assert.Contains("cannot drop below 0", ex.Message);
    }

    [Fact]
    public async Task AdjustStockAsync_SoftDeletedProduct_ThrowsInvalidOperationException()
    {
        // Arrange
        var db = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var authService = new AuthorizationService(db);
        var stockService = new StockService(db, authService);

        var product = new Product { Id = 1, Name = "Old Item", SKU = "OLD-001", IsDeleted = true };
        var warehouse = new Warehouse { Id = 1, Name = "Main Warehouse" };
        db.Products.Add(product);
        db.Warehouses.Add(warehouse);
        await db.SaveChangesAsync();

        var request = new StockAdjustmentRequest
        {
            ProductId = 1,
            WarehouseId = 1,
            Delta = 5,
            AdjustmentType = AdjustmentType.Receive
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            stockService.AdjustStockAsync(request, currentUserId: 1, currentUserRole: UserRole.Admin));

        Assert.Contains("soft-deleted product", ex.Message);
    }

    [Fact]
    public async Task AdjustStockAsync_OperatorUnassignedWarehouse_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var db = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var authService = new AuthorizationService(db);
        var stockService = new StockService(db, authService);

        var product = new Product { Id = 1, Name = "Item", SKU = "ITM-001" };
        var warehouse = new Warehouse { Id = 1, Name = "Restricted Warehouse" };
        var operatorUser = new User { Id = 2, Username = "operator1", Role = UserRole.Operator };

        db.Products.Add(product);
        db.Warehouses.Add(warehouse);
        db.Users.Add(operatorUser);
        await db.SaveChangesAsync();

        var request = new StockAdjustmentRequest
        {
            ProductId = 1,
            WarehouseId = 1,
            Delta = 5
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            stockService.AdjustStockAsync(request, currentUserId: 2, currentUserRole: UserRole.Operator));
    }
}
