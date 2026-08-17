using Microsoft.EntityFrameworkCore;
using FulfillmentInventoryPlatform.API.Models;

namespace FulfillmentInventoryPlatform.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<StockItem> StockItems { get; set; } = null!;
    public DbSet<StockAdjustment> StockAdjustments { get; set; } = null!;
    public DbSet<UserWarehouse> UserWarehouses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UserWarehouse composite key & relationships
        modelBuilder.Entity<UserWarehouse>()
            .HasKey(uw => new { uw.UserId, uw.WarehouseId });

        modelBuilder.Entity<UserWarehouse>()
            .HasOne(uw => uw.User)
            .WithMany(u => u.UserWarehouses)
            .HasForeignKey(uw => uw.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserWarehouse>()
            .HasOne(uw => uw.Warehouse)
            .WithMany(w => w.UserWarehouses)
            .HasForeignKey(uw => uw.WarehouseId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // StockItem relationships & unique constraint per product & warehouse
        modelBuilder.Entity<StockItem>()
            .HasIndex(si => new { si.ProductId, si.WarehouseId })
            .IsUnique();

        modelBuilder.Entity<StockItem>()
            .HasOne(si => si.Product)
            .WithMany(p => p.StockItems)
            .HasForeignKey(si => si.ProductId)
            .IsRequired(false);

        modelBuilder.Entity<StockItem>()
            .HasOne(si => si.Warehouse)
            .WithMany(w => w.StockItems)
            .HasForeignKey(si => si.WarehouseId)
            .IsRequired(false);

        // Product SKU unique constraint
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        // User Username & Email unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // StockAdjustment relationships
        modelBuilder.Entity<StockAdjustment>()
            .HasOne(sa => sa.StockItem)
            .WithMany(si => si.StockAdjustments)
            .HasForeignKey(sa => sa.StockItemId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockAdjustment>()
            .HasOne(sa => sa.PerformedBy)
            .WithMany(u => u.StockAdjustments)
            .HasForeignKey(sa => sa.PerformedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filters for soft delete
        modelBuilder.Entity<Warehouse>().HasQueryFilter(w => !w.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<StockItem>().HasQueryFilter(si => !si.Warehouse.IsDeleted);
        modelBuilder.Entity<UserWarehouse>().HasQueryFilter(uw => !uw.Warehouse.IsDeleted);
    }
}
