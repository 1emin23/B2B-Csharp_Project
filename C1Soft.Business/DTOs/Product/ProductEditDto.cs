namespace C1Soft.Business.DTOs.Product;

/// <summary>
/// Admin panelinden mevcut ürünü düzenlerken kullanılan DTO.
/// Görsel yükleme Controller katmanında yapılır; burada sadece URL taşınır.
/// </summary>
public class ProductEditDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string? ManufacturerCode { get; set; }
    public string? CustomCode1 { get; set; }
    public string? CustomCode2 { get; set; }

    /// <summary>
    /// Controller tarafından yeni görsel yüklenirse set edilir;
    /// null ise ExistingImageUrl korunur.
    /// </summary>
    public string? NewImageUrl { get; set; }

    /// <summary>
    /// Mevcut görsel yolu (UI'da preview için, değişmezse korunur).
    /// </summary>
    public string? ExistingImageUrl { get; set; }

    public int StockQuantity { get; set; }
    public int CriticalStockThreshold { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; }
}
