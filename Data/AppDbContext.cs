using Microsoft.EntityFrameworkCore;

using FulfillmentInventoryPlatform.API.Data.Entities;

namespace FulfillmentInventoryPlatform.Data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<Warehouse> Warehouses { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<StockItem> StockItems { get; set; }

    public DbSet<StockAdjustment> StockAdjustments { get; set; }

    public DbSet<UserWarehouse> UserWarehouses { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockItem>()
            .HasIndex(si => new { si.ProductId, si.WarehouseId })
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Warehouse>().HasQueryFilter(w => !w.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
    }
}
