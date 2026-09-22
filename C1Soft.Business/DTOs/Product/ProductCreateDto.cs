namespace C1Soft.Business.DTOs.Product;

/// <summary>
/// Admin panelinden yeni ürün oluştururken kullanılan DTO.
/// Görsel yükleme Controller katmanında yapılır; burada sadece kaydedilen URL taşınır.
/// </summary>
public class ProductCreateDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string? ManufacturerCode { get; set; }
    public string? CustomCode1 { get; set; }
    public string? CustomCode2 { get; set; }

    /// <summary>
    /// Controller tarafından IFormFile yüklendikten sonra set edilen URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    public int StockQuantity { get; set; }
    public int CriticalStockThreshold { get; set; } = 10;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
