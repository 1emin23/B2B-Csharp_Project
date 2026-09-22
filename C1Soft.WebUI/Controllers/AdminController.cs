using C1Soft.Business.DTOs.Product;
using C1Soft.Business.DTOs.User;
using C1Soft.Business.Interfaces;
using C1Soft.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C1Soft.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IUserService _userService;

    public AdminController(
        IOrderService orderService,
        IProductService productService,
        IUserService userService)
    {
        _orderService = orderService;
        _productService = productService;
        _userService = userService;
    }

    // ── 1. SİPARİŞ YÖNETİMİ ───────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetail(int id)
    {
        var order = await _orderService.GetOrderDetailAsync(id);
        if (order is null)
            return NotFound("Sipariş bulunamadı.");

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status, string? adminNote)
    {
        try
        {
            await _orderService.UpdateOrderStatusAsync(id, status, adminNote);
            TempData["SuccessMessage"] = "Sipariş durumu ve yönetici notu başarıyla güncellendi.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(OrderDetail), new { id });
    }

    // ── 2. ÜRÜN YÖNETİMİ ──────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Products(string? search)
    {
        var products = await _productService.GetAllProductsForAdminAsync(search);
        ViewBag.SearchQuery = search;
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> ProductCreate()
    {
        ViewBag.Categories = await _productService.GetCategoriesAsync();
        return View(new ProductCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductCreate(ProductCreateDto model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(model);
        }

        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                Directory.CreateDirectory(uploadsDir);
                var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.ImageUrl = $"/images/products/{fileName}";
            }

            await _productService.CreateProductAsync(model);
            TempData["SuccessMessage"] = "Yeni ürün başarıyla eklendi.";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ProductEdit(int id)
    {
        var product = await _productService.GetProductForEditAsync(id);
        if (product is null)
            return NotFound("Ürün bulunamadı.");

        ViewBag.Categories = await _productService.GetCategoriesAsync();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductEdit(ProductEditDto model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(model);
        }

        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                Directory.CreateDirectory(uploadsDir);
                var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.NewImageUrl = $"/images/products/{fileName}";
            }

            await _productService.UpdateProductAsync(model);
            TempData["SuccessMessage"] = "Ürün bilgileri başarıyla güncellendi.";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductDelete(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Ürün başarıyla pasife alındı (Soft-Delete).";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Products));
    }

    // ── 3. BAYİ / KULLANICI YÖNETİMİ ─────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Users(string? search)
    {
        var users = await _userService.GetAllUsersAsync(search);
        ViewBag.SearchQuery = search;
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> UserEdit(int id)
    {
        var user = await _userService.GetUserForEditAsync(id);
        if (user is null)
            return NotFound("Kullanıcı bulunamadı.");

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserEdit(UserEditDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _userService.UpdateUserAsync(model);
            TempData["SuccessMessage"] = "Kullanıcı/Bayi bilgileri başarıyla güncellendi.";
            return RedirectToAction(nameof(Users));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
