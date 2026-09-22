using C1Soft.Domain.Common;
using C1Soft.Domain.Enums;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Sistem kullanıcılarını (Yönetici ve Bayileri) temsil eden Entity sınıfı.
/// Case şartnamesindeki Madde 2 ve Madde 5 gereksinimlerini karşılar.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Kullanıcının adı.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının soyadı.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// İletişim ve giriş için benzersiz e-posta adresi.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Sisteme giriş için kullanılan benzersiz kullanıcı adı.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// İletişim telefon numarası.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// PBKDF2/SHA256 ile tuzlanmış güvenli şifre hash'i (Açık metin kesinlikle tutulmaz).
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Her kullanıcıya özel üretilen kriptografik tuz (Salt).
    /// </summary>
    public string PasswordSalt { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının sistemdeki yetki rolü (Admin veya Customer).
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>
    /// Kullanıcının tam adı (Ad + Soyad).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Kullanıcının aktif sepeti (Bire-bir ilişki).
    /// </summary>
    public virtual Cart? Cart { get; set; }

    /// <summary>
    /// Kullanıcının verdiği siparişlerin listesi (Bire-çok ilişki).
    /// </summary>
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
