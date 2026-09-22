using C1Soft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.DataAccess.Context;

/// <summary>
/// Entity Framework Core Veritabanı Bağlamı (Database Context).
/// Tüm B2B tabloları ve SQL Server iletişimi bu sınıf üzerinden yürütülür.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<GridColumnDefinition> GridColumnDefinitions => Set<GridColumnDefinition>();
    public DbSet<Banner> Banners => Set<Banner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // DataAccess assembly'si içerisindeki tüm Fluent API konfigürasyonlarını otomatik uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
