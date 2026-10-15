using ManagementUtility.Core.Entities;
using ManagementUtility.Core.Interfaces;
using ManagementUtility.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManagementUtility.Infrastructure.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly IDbContextFactory<ManagementUtilityDbContext> _contextFactory;

    public AssetRepository(
        IDbContextFactory<ManagementUtilityDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<Asset>> GetAllAsync(
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = dbContext.Assets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";

            query = query.Where(asset =>
                EF.Functions.Like(asset.InventoryNumber, pattern) ||
                EF.Functions.Like(asset.Name, pattern) ||
                EF.Functions.Like(asset.Category, pattern));
        }

        return await query
            .OrderBy(asset => asset.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Asset?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(asset => asset.Id == id, cancellationToken);
    }

    public async Task<bool> InventoryNumberExistsAsync(
        string inventoryNumber,
        int? exceptAssetId,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = dbContext.Assets
            .Where(asset => asset.InventoryNumber == inventoryNumber);

        if (exceptAssetId.HasValue)
        {
            query = query.Where(asset => asset.Id != exceptAssetId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Asset asset,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.Assets.Add(asset);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Asset asset,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.Assets.Update(asset);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Asset asset,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await _contextFactory.CreateDbContextAsync(cancellationToken);

        dbContext.Assets.Remove(asset);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}