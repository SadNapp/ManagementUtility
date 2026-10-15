using ManagementUtility.Core.Enums;

namespace ManagementUtility.Core.DTOs.Assets;

public class AssetListItemDto
{
    public int Id { get; set; }
    public string InventoryNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? ResponsiblePerson { get; set; }
    public AssetStatus Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Notes { get; set; }
}