using C1Soft.Domain.Common;
using C1Soft.Domain.Enums;

namespace C1Soft.Domain.Entities;

/// <summary>
/// Dinamik B2B Tablo / Grid kolon konfigürasyonlarını veritabanında tutan Entity sınıfı (Madde 6.1).
/// Bu sayede kolonların gösterimi, sırası, render stratejisi ve responsive görünürlüğü
/// kod değiştirmeden veritabanı üzerinden yönetilebilir.
/// </summary>
public class GridColumnDefinition : BaseEntity
{
    /// <summary>
    /// Tablonun ait olduğu ekran kodu (Örn: "B2B_PRODUCT_GRID").
    /// </summary>
    public string GridCode { get; set; } = "B2B_PRODUCT_GRID";

    /// <summary>
    /// Ürün modelinde karşılık gelen property / alan adı (Örn: "ImageUrl", "ProductCode", "ProductName", "StockStatusBadge", "Price").
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Tablo başlığında kullanıcıya gösterilecek metin (Örn: "Ürün Görseli", "Ürün Kodu", "Ürün Adı", "Stok Durumu").
    /// </summary>
    public string HeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Sütunun soldan sağa diziliş sırası (Order Index: 1, 2, 3...).
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Hücrenin çizilme biçimi (Text, Image, StockBadge, Price, QuantityInputWithCart, ActionModal).
    /// </summary>
    public GridRenderType RenderType { get; set; } = GridRenderType.Text;

    /// <summary>
    /// Sütun genişliği CSS değeri (Örn: "80px", "160px", "auto").
    /// </summary>
    public string? Width { get; set; }

    /// <summary>
    /// Metin hizalama seçeneği ("left", "center", "right").
    /// </summary>
    public string Alignment { get; set; } = "center";

    /// <summary>
    /// Masaüstü ekranlarda bu sütunun gösterilip gösterilmeyeceği.
    /// </summary>
    public bool IsVisibleDesktop { get; set; } = true;

    /// <summary>
    /// Tablet ekranlarda bu sütunun gösterilip gösterilmeyeceği.
    /// </summary>
    public bool IsVisibleTablet { get; set; } = true;

    /// <summary>
    /// Mobil telefon ekranlarında bu sütunun gösterilip gösterilmeyeceği.
    /// </summary>
    public bool IsVisibleMobile { get; set; } = true;
}
