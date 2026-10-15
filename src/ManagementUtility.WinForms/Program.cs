using ManagementUtility.Infrastructure;
using ManagementUtility.Infrastructure.Persistence;
using ManagementUtility.WinForms.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementUtility.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var databaseDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ManagementUtility");

        Directory.CreateDirectory(databaseDirectory);

        
        var databasePath = Path.Combine(
            databaseDirectory,
            "management-utility.db");

        var services = new ServiceCollection();

        services.AddInfrastructure($"Data Source={databasePath}");
        services.AddTransient<MainForm>();

        using var serviceProvider = services.BuildServiceProvider();

        using (var dbContext = serviceProvider
                   .GetRequiredService<IDbContextFactory<ManagementUtilityDbContext>>()
                   .CreateDbContext())
        {
            dbContext.Database.Migrate();
        }

        DatabaseSeeder.SeedDemoAssetsAsync(
                serviceProvider.GetRequiredService<
                    IDbContextFactory<ManagementUtilityDbContext>>())
            .GetAwaiter()
            .GetResult();
        
        Application.Run(serviceProvider.GetRequiredService<MainForm>());
    }
}