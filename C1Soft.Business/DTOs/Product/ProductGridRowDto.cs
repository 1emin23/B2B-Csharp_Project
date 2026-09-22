using C1Soft.Domain.Enums;

namespace C1Soft.Business.DTOs.Product;

/// <summary>
/// Dinamik B2B grid için ürün satırı verisi.
/// FieldName reflection ile bu DTO'nun property'lerine bağlanır.
/// </summary>
public class ProductGridRowDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public int CriticalStockThreshold { get; set; }
    public decimal Price { get; set; }

    /// <summary>
    /// Hesaplanan stok durumu (Var / Kritik / Yok).
    /// GridRenderType.StockBadge kolonları bu property'i kullanır.
    /// </summary>
    public StockStatus StockStatusBadge
    {
        get
        {
            if (StockQuantity <= 0) return StockStatus.OutOfStock;
            if (StockQuantity <= CriticalStockThreshold) return StockStatus.Critical;
            return StockStatus.InStock;
        }
    }

    /// <summary>
    /// Sadece grid render engine'inin "QuantityAndCartAction" ve "DetailAction"
    /// sütunlarını çizmesi için placeholder — gerçek değer kullanılmaz.
    /// </summary>
    public string? QuantityAndCartAction => null;
    public string? DetailAction => null;
}
