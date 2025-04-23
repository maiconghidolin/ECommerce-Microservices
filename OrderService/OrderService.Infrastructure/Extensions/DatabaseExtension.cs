using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Infrastructure.Extensions;

public static class DatabaseExtension
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EFContext>(options =>
        {
            options.UseNpgsql(
                configuration["Database:ConnectionString"],
                x => x.MigrationsAssembly("OrderService.Infrastructure"));
        });
    }

    public static void RunMigrations(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<EFContext>();
        dbContext.Database.Migrate();
    }
}