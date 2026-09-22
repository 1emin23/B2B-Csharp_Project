using System.Security.Claims;
using C1Soft.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C1Soft.WebUI.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null || !int.TryParse(claim.Value, out int userId))
            throw new UnauthorizedAccessException("Geçerli bir kullanıcı oturumu bulunamadı.");

        return userId;
    }

    // ── SİPARİŞİ TAMAMLA (ATOMİK CHECKOUT - MADDE 3.5 & 8) ──────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();

        try
        {
            var orderNumber = await _orderService.CreateOrderAsync(userId);
            TempData["SuccessMessage"] = $"Siparişiniz başarıyla alındı. Takip No: {orderNumber}";
            return RedirectToAction(nameof(Confirmation), new { orderNumber });
        }
        catch (InvalidOperationException ex)
        {
            // Madde 3.5: Yetersiz stok veya sepet hatası durumunda bayiye net hata mesajı göster
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Index", "Cart");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Siparişiniz oluşturulurken beklenmeyen bir hata oluştu. Lütfen tekrar deneyiniz.";
            return RedirectToAction("Index", "Cart");
        }
    }

    // ── SİPARİŞ ONAY / TEŞEKKÜR EKRANI ────────────────────────────────────────

    [HttpGet]
    public IActionResult Confirmation(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            return RedirectToAction("Index", "Product");

        ViewBag.OrderNumber = orderNumber;
        return View();
    }

    // ── BAYİNİN GEÇMİŞ SİPARİŞLERİ ─────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {
        var userId = GetCurrentUserId();
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        return View(orders);
    }

    // ── SİPARİŞ DETAY VE SNAPSHOT GÖRÜNÜMÜ (MADDE 9) ───────────────────────────

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var userId = GetCurrentUserId();
        var order = await _orderService.GetOrderDetailAsync(id, userId);

        if (order is null)
            return NotFound("Sipariş bulunamadı veya bu siparişi görüntüleme yetkiniz yok.");

        return View(order);
    }
}
