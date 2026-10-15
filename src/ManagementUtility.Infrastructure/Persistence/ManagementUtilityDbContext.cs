using ManagementUtility.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementUtility.Infrastructure.Persistence;

public class ManagementUtilityDbContext : DbContext
{
    public ManagementUtilityDbContext(
        DbContextOptions<ManagementUtilityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ManagementUtilityDbContext).Assembly);
    }
}