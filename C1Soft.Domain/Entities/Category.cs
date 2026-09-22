using C1Soft.Domain.Common;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Ürün kategorilerini temsil eden Entity sınıfı.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Kategori adı (Örn: Hırdavat, Elektrik, Endüstriyel vb.).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Kategoriye ait isteğe bağlı açıklama.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Bu kategoriye bağlı ürünlerin listesi (Navigation Property).
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
