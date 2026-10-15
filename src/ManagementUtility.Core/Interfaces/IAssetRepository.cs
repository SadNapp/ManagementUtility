using ManagementUtility.Core.Entities;

namespace ManagementUtility.Core.Interfaces;

public interface IAssetRepository
{
    Task<IReadOnlyList<Asset>> GetAllAsync(
        string? searchTerm,
        CancellationToken cancellationToken);

    Task<Asset?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<bool> InventoryNumberExistsAsync(
        string inventoryNumber,
        int? exceptAssetId,
        CancellationToken cancellationToken);

    Task AddAsync(Asset asset, CancellationToken cancellationToken);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken);
    Task DeleteAsync(Asset asset, CancellationToken cancellationToken);
}