using C1Soft.Business.DTOs.Product;
using C1Soft.Domain.Entities;

namespace C1Soft.Business.Interfaces;

public interface IProductService
{
    // --- Bayi (Customer) tarafı ---

    /// <summary>
    /// Dinamik grid kolonlarını DB'den sıralı getirir.
    /// </summary>
    Task<List<GridColumnDefinition>> GetGridColumnsAsync(string gridCode = "B2B_PRODUCT_GRID");

    /// <summary>
    /// Aktif ürünleri IQueryable SQL-level arama ile listeler.
    /// searchQuery boşsa tüm ürünleri döner.
    /// </summary>
    Task<List<ProductGridRowDto>> GetProductGridAsync(string? searchQuery);

    /// <summary>
    /// Ürün detay popup için tam veriyi getirir.
    /// </summary>
    Task<ProductDetailDto?> GetProductDetailAsync(int id);

    // --- Admin tarafı ---

    /// <summary>
    /// Admin ürün listesi (pasif ürünler dahil, arama destekli).
    /// </summary>
    Task<List<ProductDetailDto>> GetAllProductsForAdminAsync(string? searchQuery);

    Task<ProductEditDto?> GetProductForEditAsync(int id);

    Task<int> CreateProductAsync(ProductCreateDto dto);

    Task UpdateProductAsync(ProductEditDto dto);

    Task DeleteProductAsync(int id);

    Task<List<Category>> GetCategoriesAsync();
}
