using C1Soft.Business.DTOs.Product;
using C1Soft.Business.Interfaces;
using C1Soft.DataAccess.Context;
using C1Soft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.Business.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // ── Grid Kolon Konfigürasyonu ───────────────────────────────────────────────

    public async Task<List<GridColumnDefinition>> GetGridColumnsAsync(string gridCode = "B2B_PRODUCT_GRID")
    {
        return await _context.GridColumnDefinitions
            .Where(g => g.GridCode == gridCode)
            .OrderBy(g => g.OrderIndex)
            .ToListAsync();
    }

    // ── Bayi: Ürün Grid Listesi (SQL-level arama, madde 3.4) ───────────────────

    public async Task<List<ProductGridRowDto>> GetProductGridAsync(string? searchQuery)
    {
        // IQueryable — henüz SQL'e gönderilmedi, filtreleme DB tarafında yapılır
        var query = _context.Products
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var q = searchQuery.Trim();
            // Madde 3.4: Sayısal alanlar hariç tüm metinsel alanlarda SQL LIKE sorgusu
            query = query.Where(p =>
                EF.Functions.Like(p.ProductName, $"%{q}%") ||
                EF.Functions.Like(p.ProductCode, $"%{q}%") ||
                EF.Functions.Like(p.Brand, $"%{q}%") ||
                EF.Functions.Like(p.ManufacturerCode ?? "", $"%{q}%") ||
                EF.Functions.Like(p.Description ?? "", $"%{q}%") ||
                EF.Functions.Like(p.CustomCode1 ?? "", $"%{q}%") ||
                EF.Functions.Like(p.CustomCode2 ?? "", $"%{q}%"));
        }

        // SQL projeksiyon: sadece grid için gereken kolonlar çekilir
        return await query
            .OrderBy(p => p.ProductName)
            .Select(p => new ProductGridRowDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Brand = p.Brand,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                CriticalStockThreshold = p.CriticalStockThreshold,
                Price = p.Price
            })
            .ToListAsync();
    }

    // ── Bayi: Ürün Detay Modal ─────────────────────────────────────────────────

    public async Task<ProductDetailDto?> GetProductDetailAsync(int id)
    {
        return await _context.Products
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Description = p.Description,
                Brand = p.Brand,
                ManufacturerCode = p.ManufacturerCode,
                CustomCode1 = p.CustomCode1,
                CustomCode2 = p.CustomCode2,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                CriticalStockThreshold = p.CriticalStockThreshold,
                Price = p.Price,
                CategoryName = p.Category != null ? p.Category.Name : null
            })
            .FirstOrDefaultAsync();
    }

    // ── Admin: Ürün Listesi (Pasifler dahil) ───────────────────────────────────

    public async Task<List<ProductDetailDto>> GetAllProductsForAdminAsync(string? searchQuery)
    {
        // Admin için global query filter bypass — IgnoreQueryFilters() pasif ürünleri de gösterir
        var query = _context.Products
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var q = searchQuery.Trim();
            query = query.Where(p =>
                EF.Functions.Like(p.ProductName, $"%{q}%") ||
                EF.Functions.Like(p.ProductCode, $"%{q}%") ||
                EF.Functions.Like(p.Brand, $"%{q}%"));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Description = p.Description,
                Brand = p.Brand,
                ManufacturerCode = p.ManufacturerCode,
                CustomCode1 = p.CustomCode1,
                CustomCode2 = p.CustomCode2,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                CriticalStockThreshold = p.CriticalStockThreshold,
                Price = p.Price,
                CategoryName = p.Category != null ? p.Category.Name : null
            })
            .ToListAsync();
    }

    public async Task<ProductEditDto?> GetProductForEditAsync(int id)
    {
        return await _context.Products
            .IgnoreQueryFilters()
            .Where(p => p.Id == id)
            .Select(p => new ProductEditDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Description = p.Description,
                Brand = p.Brand,
                ManufacturerCode = p.ManufacturerCode,
                CustomCode1 = p.CustomCode1,
                CustomCode2 = p.CustomCode2,
                ExistingImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                CriticalStockThreshold = p.CriticalStockThreshold,
                Price = p.Price,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync();
    }

    // ── Admin: Ürün Oluştur ────────────────────────────────────────────────────

    public async Task<int> CreateProductAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            ProductCode = dto.ProductCode.Trim(),
            ProductName = dto.ProductName.Trim(),
            Description = dto.Description?.Trim(),
            Brand = dto.Brand.Trim(),
            ManufacturerCode = dto.ManufacturerCode?.Trim(),
            CustomCode1 = dto.CustomCode1?.Trim(),
            CustomCode2 = dto.CustomCode2?.Trim(),
            ImageUrl = dto.ImageUrl,
            StockQuantity = dto.StockQuantity,
            CriticalStockThreshold = dto.CriticalStockThreshold,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    // ── Admin: Ürün Güncelle ───────────────────────────────────────────────────

    public async Task UpdateProductAsync(ProductEditDto dto)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == dto.Id)
            ?? throw new InvalidOperationException($"Ürün bulunamadı. Id: {dto.Id}");

        product.ProductCode = dto.ProductCode.Trim();
        product.ProductName = dto.ProductName.Trim();
        product.Description = dto.Description?.Trim();
        product.Brand = dto.Brand.Trim();
        product.ManufacturerCode = dto.ManufacturerCode?.Trim();
        product.CustomCode1 = dto.CustomCode1?.Trim();
        product.CustomCode2 = dto.CustomCode2?.Trim();
        product.StockQuantity = dto.StockQuantity;
        product.CriticalStockThreshold = dto.CriticalStockThreshold;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;
        product.IsActive = dto.IsActive;

        // Yeni görsel yüklenmişse kaydet, yoksa mevcut yolu koru
        if (!string.IsNullOrEmpty(dto.NewImageUrl))
            product.ImageUrl = dto.NewImageUrl;

        await _context.SaveChangesAsync();
    }

    // ── Admin: Ürün Soft-Delete ────────────────────────────────────────────────

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new InvalidOperationException($"Ürün bulunamadı. Id: {id}");

        product.IsActive = false;
        await _context.SaveChangesAsync();
    }

    public Task<List<Category>> GetCategoriesAsync() =>
        _context.Categories.OrderBy(c => c.Name).ToListAsync();
}
