using C1Soft.Business.DTOs.Cart;
using C1Soft.Business.Interfaces;
using C1Soft.DataAccess.Context;
using C1Soft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.Business.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    // ── Sepeti Getir ───────────────────────────────────────────────────────────

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Where(c => c.UserId == userId)
            .Select(c => new CartDto
            {
                CartId = c.Id,
                Items = c.Items
                    .Select(ci => new CartItemDto
                    {
                        CartItemId = ci.Id,
                        ProductId = ci.ProductId,
                        ProductCode = ci.Product!.ProductCode,
                        ProductName = ci.Product.ProductName,
                        ImageUrl = ci.Product.ImageUrl,
                        UnitPrice = ci.Product.Price,
                        Quantity = ci.Quantity,
                        StockQuantity = ci.Product.StockQuantity
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return cart ?? new CartDto();
    }

    // ── Sepete Ekle (Upsert) ───────────────────────────────────────────────────

    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Adet 0'dan büyük olmalıdır.");

        // Ürünün aktif ve mevcut olup olmadığını kontrol et
        var product = await _context.Products
            .Where(p => p.Id == productId && p.IsActive)
            .Select(p => new { p.Id, p.StockQuantity })
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Ürün bulunamadı veya aktif değil.");

        if (product.StockQuantity <= 0)
            throw new InvalidOperationException("Bu ürün stokta tükenmiştir.");

        // Sepeti bul veya oluştur
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            cart = new Cart { UserId = userId, UpdatedAt = DateTime.UtcNow };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(); // Cart Id'yi al
        }

        // Ürün zaten sepette var mı? (Upsert)
        var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        int totalQuantity = (existingItem?.Quantity ?? 0) + quantity;

        if (totalQuantity > product.StockQuantity)
            throw new InvalidOperationException(
                $"Sepete eklemek istediğiniz toplam adet ({totalQuantity}), mevcut stoktan ({product.StockQuantity}) fazla.");

        if (existingItem is not null)
        {
            existingItem.Quantity = totalQuantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // ── Miktar Güncelle ────────────────────────────────────────────────────────

    public async Task UpdateQuantityAsync(int userId, int cartItemId, int newQuantity)
    {
        var item = await _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart!.UserId == userId)
            ?? throw new InvalidOperationException("Sepet kalemi bulunamadı.");

        if (newQuantity <= 0)
        {
            _context.CartItems.Remove(item);
        }
        else
        {
            if (newQuantity > item.Product!.StockQuantity)
                throw new InvalidOperationException(
                    $"Girilen adet ({newQuantity}), mevcut stoktan ({item.Product.StockQuantity}) fazla.");

            item.Quantity = newQuantity;
            item.Cart!.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    // ── Kalemi Sil ─────────────────────────────────────────────────────────────

    public async Task RemoveFromCartAsync(int userId, int cartItemId)
    {
        var item = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart!.UserId == userId)
            ?? throw new InvalidOperationException("Sepet kalemi bulunamadı.");

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    // ── Sepeti Temizle ─────────────────────────────────────────────────────────

    public async Task ClearCartAsync(int userId)
    {
        var items = await _context.CartItems
            .Where(ci => ci.Cart!.UserId == userId)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }

    // ── Navbar Badge Sayacı ────────────────────────────────────────────────────

    public async Task<int> GetCartItemCountAsync(int userId)
    {
        return await _context.CartItems
            .Where(ci => ci.Cart!.UserId == userId)
            .SumAsync(ci => (int?)ci.Quantity) ?? 0;
    }
}
