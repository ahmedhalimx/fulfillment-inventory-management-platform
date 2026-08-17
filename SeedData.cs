using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using FulfillmentInventoryPlatform.API.Enums;
using FulfillmentInventoryPlatform.API.Models;

namespace FulfillmentInventoryPlatform.API.Data;

public static class SeedData
{
    public static async Task Initialize(AppDbContext context)
    {
        // Check if already seeded
        if (await context.Users.AnyAsync()) return;

        // Add categories
        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Electronic devices", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Furniture", Description = "Home and office furniture", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Clothing", Description = "Apparel and accessories", CreatedAt = DateTime.UtcNow }
        };
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Add warehouses
        var warehouses = new[]
        {
            new Warehouse { Name = "Main Warehouse", Location = "123 Main St, City", CreatedAt = DateTime.UtcNow },
            new Warehouse { Name = "East Warehouse", Location = "456 East Ave, City", CreatedAt = DateTime.UtcNow },
            new Warehouse { Name = "West Warehouse", Location = "789 West Blvd, City", CreatedAt = DateTime.UtcNow }
        };
        await context.Warehouses.AddRangeAsync(warehouses);
        await context.SaveChangesAsync();

        // Add products
        var products = new[]
        {
            new Product { Name = "Smartphone", Description = "Latest model", SKU = "PH-001", CategoryId = categories[0].Id, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Laptop", Description = "High performance", SKU = "LP-002", CategoryId = categories[0].Id, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Desk", Description = "Wooden desk", SKU = "DK-003", CategoryId = categories[1].Id, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Chair", Description = "Ergonomic chair", SKU = "CH-004", CategoryId = categories[1].Id, CreatedAt = DateTime.UtcNow },
            new Product { Name = "T-Shirt", Description = "Cotton t-shirt", SKU = "TS-005", CategoryId = categories[2].Id, CreatedAt = DateTime.UtcNow }
        };
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // Add stock items for each product in each warehouse
        var stockItems = new List<StockItem>();
        var rnd = new Random(42);
        foreach (var product in products)
        {
            foreach (var warehouse in warehouses)
            {
                stockItems.Add(new StockItem
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = rnd.Next(5, 50),
                    LastUpdatedAt = DateTime.UtcNow
                });
            }
        }
        await context.StockItems.AddRangeAsync(stockItems);
        await context.SaveChangesAsync();

        // Create users with UserRole enum
        var adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        var managerPassword = BCrypt.Net.BCrypt.HashPassword("Manager@123");
        var operatorPassword = BCrypt.Net.BCrypt.HashPassword("Operator@123");

        var admin = new User
        {
            Username = "admin",
            Email = "admin@test.com",
            PasswordHash = adminPassword,
            FullName = "Admin User",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };
        var manager = new User
        {
            Username = "manager",
            Email = "manager@test.com",
            PasswordHash = managerPassword,
            FullName = "Manager User",
            Role = UserRole.Manager,
            CreatedAt = DateTime.UtcNow
        };
        var operator1 = new User
        {
            Username = "operator1",
            Email = "operator1@test.com",
            PasswordHash = operatorPassword,
            FullName = "Operator One",
            Role = UserRole.Operator,
            CreatedAt = DateTime.UtcNow
        };
        var operator2 = new User
        {
            Username = "operator2",
            Email = "operator2@test.com",
            PasswordHash = operatorPassword,
            FullName = "Operator Two",
            Role = UserRole.Operator,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(admin, manager, operator1, operator2);
        await context.SaveChangesAsync();

        // Assign operator1 to Main and East; operator2 to West
        var userWarehouses = new[]
        {
            new UserWarehouse { UserId = operator1.Id, WarehouseId = warehouses[0].Id },
            new UserWarehouse { UserId = operator1.Id, WarehouseId = warehouses[1].Id },
            new UserWarehouse { UserId = operator2.Id, WarehouseId = warehouses[2].Id }
        };
        await context.UserWarehouses.AddRangeAsync(userWarehouses);
        await context.SaveChangesAsync();

        // Add initial stock adjustments for history with AdjustmentType enum
        var adjustments = new List<StockAdjustment>();
        var stockItemList = await context.StockItems.ToListAsync();
        foreach (var si in stockItemList.Take(10))
        {
            var initialQty = si.Quantity;
            adjustments.Add(new StockAdjustment
            {
                StockItemId = si.Id,
                PreviousQuantity = 0,
                NewQuantity = initialQty,
                QuantityDelta = initialQty,
                AdjustmentType = AdjustmentType.Receive,
                Note = "Initial seed inventory intake",
                PerformedByUserId = admin.Id,
                PerformedAt = DateTime.UtcNow.AddDays(-1)
            });
        }
        await context.StockAdjustments.AddRangeAsync(adjustments);
        await context.SaveChangesAsync();
    }
}
