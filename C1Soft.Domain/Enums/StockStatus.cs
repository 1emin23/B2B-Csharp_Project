namespace C1Soft.Domain.Enums;

/// <summary>
/// Ürünün kritik stok eşiğine göre hesaplanan görsel durum rozetini tanımlar (Madde 6.1).
/// </summary>
public enum StockStatus
{
    /// <summary>
    /// Stok miktarı kritik eşiğin üzerinde (Yeşil rozet: "Var").
    /// </summary>
    InStock = 1,

    /// <summary>
    /// Stok miktarı kritik eşiğin altında ancak sıfırdan büyük (Sarı rozet: "Kritik").
    /// </summary>
    Critical = 2,

    /// <summary>
    /// Stok miktarı sıfır veya negatif (Kırmızı rozet: "Yok").
    /// </summary>
    OutOfStock = 3
}
