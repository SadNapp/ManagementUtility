using ManagementUtility.Core.Interfaces;
using ManagementUtility.Core.Services;
using ManagementUtility.Infrastructure.Persistence;
using ManagementUtility.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementUtility.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextFactory<ManagementUtilityDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddTransient<IAssetRepository, AssetRepository>();
        services.AddTransient<IAssetService, AssetService>();

        return services;
    }
}