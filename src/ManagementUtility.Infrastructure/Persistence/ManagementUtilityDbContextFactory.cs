using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ManagementUtility.Infrastructure.Persistence;

public class ManagementUtilityDbContextFactory
    : IDesignTimeDbContextFactory<ManagementUtilityDbContext>
{
    public ManagementUtilityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ManagementUtilityDbContext>()
            .UseSqlite("Data Source=management-utility-design.db")
            .Options;

        return new ManagementUtilityDbContext(options);
    }
}