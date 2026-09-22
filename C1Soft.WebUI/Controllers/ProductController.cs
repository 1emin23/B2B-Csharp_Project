using C1Soft.Business.Interfaces;
using C1Soft.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace C1Soft.WebUI.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    // ── B2B DİNAMİK ÜRÜN GRİDİ (KATALOG) ───────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var columns = await _productService.GetGridColumnsAsync("B2B_PRODUCT_GRID");
        var products = await _productService.GetProductGridAsync(search);

        var viewModel = new ProductIndexViewModel
        {
            Columns = columns,
            Products = products,
            SearchQuery = search
        };

        return View(viewModel);
    }

    // ── ÜRÜN DETAY MODAL (AJAX/JSON) ──────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _productService.GetProductDetailAsync(id);
        if (product is null)
            return NotFound(new { message = "Ürün bulunamadı veya satışta değil." });

        return Json(product);
    }
}
