using ManagementUtility.Core.DTOs.Assets;

namespace ManagementUtility.Core.Interfaces;

public interface IAssetService
{
    Task<IReadOnlyList<AssetListItemDto>> GetAllAsync(
        string? searchTerm,
        CancellationToken cancellationToken);

    Task<AssetListItemDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task CreateAsync(
        CreateAssetRequest request,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        UpdateAssetRequest request,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}