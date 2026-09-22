namespace C1Soft.Domain.Enums;

/// <summary>
/// Siparişin yaşam döngüsü durumlarını tanımlar.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Sipariş oluşturuldu, yönetici incelemesi bekleniyor.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Sipariş yönetici tarafından onaylandı.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Sipariş yönetici tarafından reddedildi.
    /// </summary>
    Rejected = 3
}
