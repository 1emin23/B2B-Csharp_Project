namespace C1Soft.Domain.Enums;

/// <summary>
/// Kullanıcı yetki ve erişim rollerini tanımlar.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Sistem Yöneticisi: Ürün, Kullanıcı ve Sipariş yönetim paneline tam erişim.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Bayi / Müşteri: B2B kataloğuna erişim, doğrudan sipariş ve sipariş takibi.
    /// </summary>
    Customer = 2
}
