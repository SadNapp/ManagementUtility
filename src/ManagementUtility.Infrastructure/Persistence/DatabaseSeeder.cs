using ManagementUtility.Core.Entities;
using ManagementUtility.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace ManagementUtility.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedDemoAssetsAsync(
        IDbContextFactory<ManagementUtilityDbContext> contextFactory)
    {
        await using var dbContext =
            await contextFactory.CreateDbContextAsync();

        var existingDemoCount = await dbContext.Assets
            .CountAsync(asset => asset.InventoryNumber.StartsWith("MU-"));

        if (existingDemoCount >= 45)
        {
            return;
        }

        var equipment = new[]
        {
            ("Перфоратор Bosch", "Електроінструмент"),
            ("Бетонозмішувач", "Будівельна техніка"),
            ("Драбина алюмінієва", "Інвентар"),
            ("Генератор дизельний", "Електрообладнання"),
            ("Шуруповерт Makita", "Електроінструмент")
        };

        var locations = new[]
        {
            "Склад №1",
            "Будівельний майданчик А",
            "Будівельний майданчик Б",
            "Сервісна зона",
            "Офіс"
        };

        var statuses = Enum.GetValues<AssetStatus>();
        var assets = new List<Asset>();
        var now = DateTime.UtcNow;

        for (var i = existingDemoCount + 1; i <= 45; i++)
        {
            var item = equipment[(i - 1) % equipment.Length];

            assets.Add(new Asset
            {
                InventoryNumber = $"MU-{i:0000}",
                Name = $"{item.Item1} #{i:00}",
                Category = item.Item2,
                Location = locations[(i - 1) % locations.Length],
                ResponsiblePerson = $"Працівник {(i - 1) % 8 + 1}",
                Status = statuses[(i - 1) % statuses.Length],
                PurchaseDate = now.AddDays(-i * 30),
                PurchasePrice = 2500m + i * 1375m,
                Notes = i % 3 == 0 ? "Потребує планового огляду" : null,
                CreatedAtUtc = now
            });
        }

        await dbContext.Assets.AddRangeAsync(assets);
        await dbContext.SaveChangesAsync();
    }
}