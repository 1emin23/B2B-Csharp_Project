using C1Soft.Domain.Common;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Sepet içerisindeki tekil bir ürün satırını temsil eden Entity sınıfı (Madde 7).
/// </summary>
public class CartItem : BaseEntity
{
    /// <summary>
    /// Bağlı olduğu sepetin kimlik numarası (Foreign Key).
    /// </summary>
    public int CartId { get; set; }

    /// <summary>
    /// Sepet nesne referansı.
    /// </summary>
    public virtual Cart? Cart { get; set; }

    /// <summary>
    /// Sepete eklenen ürünün kimlik numarası (Foreign Key).
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Ürün nesne referansı.
    /// </summary>
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Sipariş edilmek istenen ürün adedi (Sıfırdan büyük olmalı).
    /// </summary>
    public int Quantity { get; set; }
}
