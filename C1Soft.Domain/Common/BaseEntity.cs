namespace C1Soft.Domain.Common;

/// <summary>
/// Tüm veritabanı Entity sınıfları için ortak temel sınıf (Base Entity).
/// Kod tekrarını önler; Id, oluşturulma tarihi ve aktiflik durumunu standartlaştırır.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Benzersiz birincil anahtar (Primary Key).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Kaydın aktif/pasif durumu (Soft-delete ve durum yönetimi için).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Kaydın sisteme eklendiği UTC zaman damgası.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
