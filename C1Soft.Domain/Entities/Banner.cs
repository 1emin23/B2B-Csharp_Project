using C1Soft.Domain.Common;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Ana sayfa slider/banner alanında gösterilen kampanya, duyuru ve reklam içeriklerini temsil eden Entity sınıfı (Madde 4.1).
/// </summary>
public class Banner : BaseEntity
{
    /// <summary>
    /// Slider başlığı veya kampanya adı.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Slider alt başlığı veya açıklama metni.
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Kampanya görselinin dosya yolu (/images/banners/slider1.jpg).
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı banner'a tıkladığında yönlendirileceği sayfa linki (İsteğe bağlı).
    /// </summary>
    public string? RedirectUrl { get; set; }

    /// <summary>
    /// Slider içinde gösterilme sırası.
    /// </summary>
    public int OrderIndex { get; set; } = 0;
}
