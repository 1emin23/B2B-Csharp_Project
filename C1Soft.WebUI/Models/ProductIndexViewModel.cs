using C1Soft.Business.DTOs.Product;
using C1Soft.Domain.Entities;

namespace C1Soft.WebUI.Models;

public class ProductIndexViewModel
{
    public List<GridColumnDefinition> Columns { get; set; } = new();
    public List<ProductGridRowDto> Products { get; set; } = new();
    public string? SearchQuery { get; set; }
}
