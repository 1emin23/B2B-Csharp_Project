using C1Soft.Domain.Enums;

namespace C1Soft.Business.DTOs.Order;

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CustomerUsername { get; set; }
}
