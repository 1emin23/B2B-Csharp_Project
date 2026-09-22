using C1Soft.Business.DTOs.Cart;

namespace C1Soft.Business.Interfaces;

public interface ICartService
{
    /// <summary>
    /// Kullanıcının sepetini ürün fiyat ve stok bilgileriyle birlikte getirir.
    /// Sepet yoksa boş CartDto döner.
    /// </summary>
    Task<CartDto> GetCartAsync(int userId);

    /// <summary>
    /// Ürünü sepete ekler. Ürün zaten sepette varsa miktarı artırır (upsert).
    /// Stok miktarını aşan ekleme girişimi exception fırlatır.
    /// </summary>
    Task AddToCartAsync(int userId, int productId, int quantity);

    /// <summary>
    /// Sepetteki ürünün miktarını günceller. Miktar 0 veya negatifse ürünü siler.
    /// </summary>
    Task UpdateQuantityAsync(int userId, int cartItemId, int newQuantity);

    /// <summary>
    /// Belirtilen kalemi sepetten kaldırır.
    /// </summary>
    Task RemoveFromCartAsync(int userId, int cartItemId);

    /// <summary>
    /// Sepetin tüm kalemlerini siler (sipariş sonrası çağrılır).
    /// </summary>
    Task ClearCartAsync(int userId);

    /// <summary>
    /// Sepetteki toplam ürün adedi (navbar badge için).
    /// </summary>
    Task<int> GetCartItemCountAsync(int userId);
}
