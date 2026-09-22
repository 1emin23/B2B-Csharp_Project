namespace C1Soft.Domain.Enums;

/// <summary>
/// Dinamik B2B grid tablosundaki hücrenin nasıl çizileceğini (render edileceğini) belirler (Madde 6.1).
/// Veritabanından yönetilen esnek kolon mimarisi sağlar.
/// </summary>
public enum GridRenderType
{
    /// <summary>
    /// Düz metin olarak basılır (Ürün adı, marka vb.).
    /// </summary>
    Text = 1,

    /// <summary>
    /// Küçük boy görsel olarak basılır (Ürün resmi).
    /// </summary>
    Image = 2,

    /// <summary>
    /// Kritik eşiğe göre hesaplanan renkli rozet olarak basılır (Var/Kritik/Yok).
    /// </summary>
    StockBadge = 3,

    /// <summary>
    /// Para birimi formatında basılır (₺1.250,00).
    /// </summary>
    Price = 4,

    /// <summary>
    /// Adet giriş kutusu ve doğrudan "Sepete Ekle" aksiyon butonu olarak basılır.
    /// </summary>
    QuantityInputWithCart = 5,

    /// <summary>
    /// Tıklandığında ürün detay Popup (Modal) penceresini açan aksiyon butonu.
    /// </summary>
    ActionModal = 6
}
