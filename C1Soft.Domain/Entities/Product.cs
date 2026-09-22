using C1Soft.Domain.Common;
using C1Soft.Domain.Enums;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Sistemde satışa sunulan B2B ürünlerini temsil eden Entity sınıfı.
/// Case şartnamesindeki 1.1 ve 6.1 maddelerindeki tüm asgari alanları içerir.
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Benzersiz ürün kodu (Örn: "PRD-1001", "ELK-450").
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// Ürün ticari adı.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Ürünün detaylı teknik veya genel açıklaması.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ürünün markası (Örn: Bosch, Siemens, Philips).
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Üretici fabrika/parça kodu (OEM Kodu).
    /// </summary>
    public string? ManufacturerCode { get; set; }

    /// <summary>
    /// B2B ERP entegrasyonları için Özel Kod 1.
    /// </summary>
    public string? CustomCode1 { get; set; }

    /// <summary>
    /// B2B ERP entegrasyonları için Özel Kod 2.
    /// </summary>
    public string? CustomCode2 { get; set; }

    /// <summary>
    /// Ürünün küçük/büyük boy görsel dosya yolu.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Fiziksel depodaki mevcut stok adedi (Negatif olamaz).
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Ürün bazında tanımlanan kritik stok seviye eşiği (Madde 6.1).
    /// Stok bu değerin altına düştüğünde sarı ("Kritik") rozeti yanar.
    /// </summary>
    public int CriticalStockThreshold { get; set; } = 10;

    /// <summary>
    /// Ürünün B2B güncel liste satış fiyatı (KDV hariç/dahil).
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Bağlı olduğu kategorinin kimlik numarası (Foreign Key).
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Kategori nesne referansı (Navigation Property).
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Ürünün mevcut stok miktarı ve kritik stok eşiğine göre hesaplanan B2B durum rozeti (Madde 6.1).
    /// Veritabanında ayrı kolon olarak saklanmaz, nesne üzerinden dinamik hesaplanır.
    /// </summary>
    public StockStatus StockStatus
    {
        get
        {
            if (StockQuantity <= 0)
                return StockStatus.OutOfStock;

            if (StockQuantity <= CriticalStockThreshold)
                return StockStatus.Critical;

            return StockStatus.InStock;
        }
    }
}
