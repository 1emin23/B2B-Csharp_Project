using C1Soft.Domain.Enums;

namespace C1Soft.Business.DTOs.Product;

/// <summary>
/// Ürün detay popup (modal) için kullanılan tam veri seti.
/// </summary>
public class ProductDetailDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string? ManufacturerCode { get; set; }
    public string? CustomCode1 { get; set; }
    public string? CustomCode2 { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public int CriticalStockThreshold { get; set; }
    public decimal Price { get; set; }
    public string? CategoryName { get; set; }
    public StockStatus StockStatus
    {
        get
        {
            if (StockQuantity <= 0) return StockStatus.OutOfStock;
            if (StockQuantity <= CriticalStockThreshold) return StockStatus.Critical;
            return StockStatus.InStock;
        }
    }
}
