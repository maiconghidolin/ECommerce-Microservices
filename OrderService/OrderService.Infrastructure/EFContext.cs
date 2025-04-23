using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using System.Reflection;

namespace OrderService.Infrastructure;

public class EFContext : DbContext
{
    public EFContext(DbContextOptions<EFContext> options) : base(options) { }

    #region Configurations

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(EFContext)));

        base.OnModelCreating(modelBuilder);
    }

    #endregion

    #region DbSet

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<PaymentData> PaymentData { get; set; }

    #endregion
}
