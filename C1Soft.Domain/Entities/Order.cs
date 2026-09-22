using C1Soft.Domain.Common;
using C1Soft.Domain.Enums;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Bayi tarafından oluşturulan sipariş üst bilgilerini temsil eden Entity sınıfı (Madde 3 ve 9).
/// </summary>
public class Order : BaseEntity
{
    /// <summary>
    /// Kullanıcıya ve yöneticiye gösterilen benzersiz sipariş takip numarası (Örn: ORD-20260922-0001).
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Siparişi oluşturan kullanıcının kimlik numarası (Foreign Key).
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Siparişi oluşturan kullanıcı nesnesi referansı.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Siparişin verildiği UTC tarih ve saat.
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Siparişin güncel onay durumu (Pending, Approved, Rejected).
    /// Yönetici tarafından değiştirildiğinde doğrudan bayi ekranına yansır.
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Siparişin tüm kalemler dahil genel toplam tutarı.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Yönetici tarafından sipariş onaylanırken veya reddedilirken girilen isteğe bağlı açıklama notu.
    /// </summary>
    public string? AdminNote { get; set; }

    /// <summary>
    /// Siparişe ait kalemlerin listesi (Navigation Property).
    /// </summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
