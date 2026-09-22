using C1Soft.Domain.Common;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Kullanıcının aktif alışveriş sepetini temsil eden Entity sınıfı (Madde 7).
/// </summary>
public class Cart : BaseEntity
{
    /// <summary>
    /// Sepetin sahibi olan kullanıcının kimlik numarası (Foreign Key & Unique).
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Sepet sahibi kullanıcı nesne referansı.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Sepetin son güncellenme tarihi.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Sepetteki ürün kalemleri listesi.
    /// </summary>
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
