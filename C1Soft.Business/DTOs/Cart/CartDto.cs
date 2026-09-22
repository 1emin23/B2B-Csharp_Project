namespace C1Soft.Business.DTOs.Cart;

public class CartDto
{
    public int CartId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal GrandTotal => Items.Sum(i => i.LineTotal);
    public int TotalItemCount => Items.Sum(i => i.Quantity);
    public bool HasStockWarnings => Items.Any(i => i.HasStockWarning);
}
