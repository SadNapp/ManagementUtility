using ManagementUtility.Core.DTOs.Assets;
using ManagementUtility.Core.Entities;
using ManagementUtility.Core.Interfaces;

namespace ManagementUtility.Core.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;

    public AssetService(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<IReadOnlyList<AssetListItemDto>> GetAllAsync(
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var assets = await _assetRepository.GetAllAsync(
            searchTerm,
            cancellationToken);

        return assets.Select(MapToDto).ToList();
    }

    public async Task<AssetListItemDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(
            id,
            cancellationToken);

        return asset is null ? null : MapToDto(asset);
    }

    public async Task CreateAsync(
        CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateAsync(
            request.InventoryNumber,
            request.Name,
            request.Category,
            request.PurchasePrice,
            exceptAssetId: null,
            cancellationToken);

        var asset = new Asset
        {
            InventoryNumber = request.InventoryNumber.Trim(),
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Location = request.Location.Trim(),
            ResponsiblePerson = CleanOptionalValue(request.ResponsiblePerson),
            Status = request.Status,
            PurchaseDate = request.PurchaseDate,
            PurchasePrice = request.PurchasePrice,
            Notes = CleanOptionalValue(request.Notes),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _assetRepository.AddAsync(asset, cancellationToken);
    }

    public async Task UpdateAsync(
        int id,
        UpdateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (asset is null)
        {
            throw new KeyNotFoundException($"Актив з ID {id} не знайдено.");
        }

        await ValidateAsync(
            request.InventoryNumber,
            request.Name,
            request.Category,
            request.PurchasePrice,
            exceptAssetId: id,
            cancellationToken);

        asset.InventoryNumber = request.InventoryNumber.Trim();
        asset.Name = request.Name.Trim();
        asset.Category = request.Category.Trim();
        asset.Location = request.Location.Trim();
        asset.ResponsiblePerson = CleanOptionalValue(request.ResponsiblePerson);
        asset.Status = request.Status;
        asset.PurchaseDate = request.PurchaseDate;
        asset.PurchasePrice = request.PurchasePrice;
        asset.Notes = CleanOptionalValue(request.Notes);
        

        await _assetRepository.UpdateAsync(asset, cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (asset is null)
        {
            throw new KeyNotFoundException($"Актив з ID {id} не знайдено.");
        }

        await _assetRepository.DeleteAsync(asset, cancellationToken);
    }

    private async Task ValidateAsync(
        string inventoryNumber,
        string name,
        string category,
        decimal? purchasePrice,
        int? exceptAssetId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Вкажи інвентарний номер.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Вкажи назву активу.");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Вкажи категорію активу.");
        }

        if (purchasePrice is < 0)
        {
            throw new ArgumentException("Ціна не може бути від’ємною.");
        }

        var alreadyExists =
            await _assetRepository.InventoryNumberExistsAsync(
                inventoryNumber.Trim(),
                exceptAssetId,
                cancellationToken);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "Актив з таким інвентарним номером вже існує.");
        }
    }

    private static string? CleanOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static AssetListItemDto MapToDto(Asset asset)
    {
        return new AssetListItemDto
        {
            Id = asset.Id,
            InventoryNumber = asset.InventoryNumber,
            Name = asset.Name,
            Category = asset.Category,
            Location = asset.Location,
            ResponsiblePerson = asset.ResponsiblePerson,
            Status = asset.Status,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice,
            Notes = asset.Notes
        };
    }
}