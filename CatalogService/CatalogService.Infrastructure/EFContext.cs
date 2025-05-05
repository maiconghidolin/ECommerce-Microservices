using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CatalogService.Infrastructure;

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

    public DbSet<Product> Products { get; set; }

    #endregion
}

