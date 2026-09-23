using C1Soft.Business.DTOs.Order;
using C1Soft.Business.Interfaces;
using C1Soft.DataAccess.Context;
using C1Soft.Domain.Entities;
using C1Soft.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.Business.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    // ── Sipariş Oluştur (Atomik TX + Stok Kontrolü) ────────────────────────────

    public async Task<string> CreateOrderAsync(int userId)
    {
        // Sepet ve kalemleri tek sorguda çek (ürün stok bilgisiyle)
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null || !cart.Items.Any())
            throw new InvalidOperationException("Sepetiniz boş. Sipariş oluşturulamaz.");

        // ── TRANSACTION BAŞLAT ────────────────────────────────────────────────
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Backend stok kontrolü (madde 3.5 — çift kontrol)
            foreach (var item in cart.Items)
            {
                var product = item.Product
                    ?? throw new InvalidOperationException($"Ürün bilgisi okunamadı (CartItem Id: {item.Id}).");

                if (!product.IsActive)
                    throw new InvalidOperationException(
                        $"\"{product.ProductName}\" ürünü artık satışta değil. Lütfen sepetinizi güncelleyin.");

                if (item.Quantity > product.StockQuantity)
                    throw new InvalidOperationException(
                        $"\"{product.ProductName}\" için yeterli stok bulunmamaktadır. Mevcut stok: {product.StockQuantity}.");
            }

            // ── Sipariş kaydını oluştur (Unique OrderNumber anında atanır) ──────
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0 // aşağıda hesaplanacak
            };

            // ── Kalemler, stok düşümü ve snapshot ────────────────────────────
            decimal totalAmount = 0;

            foreach (var item in cart.Items)
            {
                var product = item.Product!;

                // Stok düş
                product.StockQuantity -= item.Quantity;

                // Snapshot kaydı (fiyat/isim değişse de korunur)
                var orderItem = new OrderItem
                {
                    Order = order,
                    ProductId = product.Id,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    TotalPrice = product.Price * item.Quantity
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;
            }

            order.TotalAmount = totalAmount;
            _context.Orders.Add(order);

            // ── Sepeti temizle ────────────────────────────────────────────────
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return order.OrderNumber;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ── Bayinin Kendi Siparişleri ──────────────────────────────────────────────

    public async Task<List<OrderSummaryDto>> GetOrdersByUserAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CustomerFullName = o.User != null ? o.User.FirstName + " " + o.User.LastName : null,
                CustomerUsername = o.User != null ? o.User.Username : null
            })
            .ToListAsync();
    }

    // ── Admin: Tüm Siparişler ──────────────────────────────────────────────────

    public async Task<List<OrderSummaryDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CustomerFullName = o.User != null ? o.User.FirstName + " " + o.User.LastName : null,
                CustomerUsername = o.User != null ? o.User.Username : null
            })
            .ToListAsync();
    }

    // ── Sipariş Detayı ────────────────────────────────────────────────────────

    public async Task<OrderDetailDto?> GetOrderDetailAsync(int orderId, int? userId = null)
    {
        var query = _context.Orders.AsQueryable();

        // Bayi yalnızca kendi siparişini görebilir
        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        return await query
            .Where(o => o.Id == orderId)
            .Select(o => new OrderDetailDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                AdminNote = o.AdminNote,
                CustomerFullName = o.User != null ? o.User.FirstName + " " + o.User.LastName : null,
                CustomerEmail = o.User != null ? o.User.Email : null,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductCode = oi.ProductCode,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    ImageUrl = oi.Product != null ? oi.Product.ImageUrl : null
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // ── Admin: Durum Güncelle ─────────────────────────────────────────────────

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? adminNote)
    {
        var order = await _context.Orders.FindAsync(orderId)
            ?? throw new InvalidOperationException($"Sipariş bulunamadı. Id: {orderId}");

        order.Status = newStatus;
        order.AdminNote = adminNote?.Trim();
        await _context.SaveChangesAsync();
    }
}
