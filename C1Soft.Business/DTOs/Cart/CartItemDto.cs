namespace C1Soft.Business.DTOs.Cart;

public class CartItemDto
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int StockQuantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;

    /// <summary>
    /// Sepetteki adet mevcut stoku aşıyorsa uyarı göstermek için.
    /// </summary>
    public bool HasStockWarning => Quantity > StockQuantity;
}
