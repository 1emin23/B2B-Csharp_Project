using C1Soft.Business.DTOs.Order;
using C1Soft.Domain.Enums;

namespace C1Soft.Business.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Kullanıcının aktif sepetini siparişe dönüştürür.
    /// DB transaction içinde stok kontrolü, stok düşümü, snapshot kayıt ve sepet temizliği yapar.
    /// Yetersiz stok varsa InvalidOperationException fırlatır (mesaj kullanıcıya gösterilebilir).
    /// </summary>
    Task<string> CreateOrderAsync(int userId);

    /// <summary>
    /// Bayinin kendi siparişlerini görüntülemesi için.
    /// </summary>
    Task<List<OrderSummaryDto>> GetOrdersByUserAsync(int userId);

    /// <summary>
    /// Admin paneli için tüm siparişleri listeler.
    /// </summary>
    Task<List<OrderSummaryDto>> GetAllOrdersAsync();

    /// <summary>
    /// Sipariş detayını getirir. userId null ise admin erişimi (yetki kontrolü yok).
    /// userId verilirse sadece o kullanıcının siparişi döner.
    /// </summary>
    Task<OrderDetailDto?> GetOrderDetailAsync(int orderId, int? userId = null);

    /// <summary>
    /// Admin tarafından sipariş durumunu günceller.
    /// </summary>
    Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? adminNote);
}
