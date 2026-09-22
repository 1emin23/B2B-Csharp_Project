using C1Soft.Domain.Common;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Sipariş kalemlerini temsil eden Entity sınıfı (Madde 3 ve 9).
/// B2B gereksinimi: Sipariş anındaki fiyat, ürün kodu ve adını "Snapshot" olarak dondurur;
/// ürünün fiyatı veya bilgisi sonradan değişse dahi geçmiş sipariş kaydı bozulmaz.
/// </summary>
public class OrderItem : BaseEntity
{
    /// <summary>
    /// Bağlı olduğu siparişin kimlik numarası (Foreign Key).
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Sipariş nesne referansı.
    /// </summary>
    public virtual Order? Order { get; set; }

    /// <summary>
    /// Satın alınan orijinal ürünün kimlik numarası (Foreign Key).
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Orijinal ürün referansı.
    /// </summary>
    public virtual Product? Product { get; set; }

    /// <summary>
    /// [SNAPSHOT] Siparişin oluşturulduğu andaki ürün kodu.
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// [SNAPSHOT] Siparişin oluşturulduğu andaki ürün ticari adı.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// [SNAPSHOT] Siparişin oluşturulduğu andaki ürün birim satış fiyatı.
    /// Ürünün genel liste fiyatı sonradan artsa/azalsa bile bu değer sabit kalır.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Sipariş edilen ürün adedi.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Satır toplam tutarı (UnitPrice * Quantity).
    /// </summary>
    public decimal TotalPrice { get; set; }
}
