using System.Security.Claims;
using C1Soft.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C1Soft.WebUI.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null || !int.TryParse(claim.Value, out int userId))
            throw new UnauthorizedAccessException("Geçerli bir kullanıcı oturumu bulunamadı.");

        return userId;
    }

    // ── SEPET SAYFASI (VIEW) ───────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var cart = await _cartService.GetCartAsync(userId);
        return View(cart);
    }

    // ── SEPETE EKLE (AJAX / POST) ──────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        if (quantity <= 0)
            return Json(new { success = false, message = "Adet en az 1 olmalıdır." });

        try
        {
            var userId = GetCurrentUserId();
            await _cartService.AddToCartAsync(userId, productId, quantity);
            var totalCount = await _cartService.GetCartItemCountAsync(userId);

            return Json(new
            {
                success = true,
                message = "Ürün sepete başarıyla eklendi.",
                cartCount = totalCount
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ── ADET GÜNCELLE (AJAX / POST) ────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _cartService.UpdateQuantityAsync(userId, cartItemId, quantity);
            var cart = await _cartService.GetCartAsync(userId);

            return Json(new
            {
                success = true,
                message = "Sepet güncellendi.",
                cartCount = cart.TotalItemCount,
                grandTotal = cart.GrandTotal.ToString("N2") + " ₺"
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ── KALEMİ SİL (AJAX / POST) ───────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _cartService.RemoveFromCartAsync(userId, cartItemId);
            var cart = await _cartService.GetCartAsync(userId);

            return Json(new
            {
                success = true,
                message = "Ürün sepetten çıkarıldı.",
                cartCount = cart.TotalItemCount,
                grandTotal = cart.GrandTotal.ToString("N2") + " ₺"
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ── SEPETİ BOŞALT (POST) ───────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var userId = GetCurrentUserId();
        await _cartService.ClearCartAsync(userId);
        TempData["SuccessMessage"] = "Sepetiniz tamamen temizlendi.";
        return RedirectToAction(nameof(Index));
    }

    // ── NAVBAR ROZET SAYAÇ ENDPOINT'İ (AJAX / GET) ────────────────────────────

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCartCount()
    {
        if (User.Identity?.IsAuthenticated != true || User.IsInRole("Admin"))
            return Json(new { count = 0 });

        var userId = GetCurrentUserId();
        var count = await _cartService.GetCartItemCountAsync(userId);
        return Json(new { count });
    }
}
