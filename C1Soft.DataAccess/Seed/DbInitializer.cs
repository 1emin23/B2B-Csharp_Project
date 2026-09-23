using C1Soft.DataAccess.Context;
using C1Soft.DataAccess.Security;
using C1Soft.Domain.Entities;
using C1Soft.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace C1Soft.DataAccess.Seed;

/// <summary>
/// Uygulama ilk ayağa kalktığında veritabanını kontrol edip
/// gerekli varsayılan tabloları ve test verilerini tohumlayan (seed eden) sınıf.
/// </summary>
public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        // 1. Veritabanının oluşturulduğundan ve migration'ların uygulandığından emin ol
        await context.Database.MigrateAsync();

        // 2. Dinamik Grid Kolon Konfigürasyonlarını Tohumla (Madde 6.1)
        if (!await context.GridColumnDefinitions.AnyAsync())
        {
            var gridColumns = new List<GridColumnDefinition>
            {
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "ImageUrl",
                    HeaderName = "Görsel",
                    OrderIndex = 1,
                    RenderType = GridRenderType.Image,
                    Width = "80px",
                    Alignment = "center",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = true
                },
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "ProductCode",
                    HeaderName = "Ürün Kodu",
                    OrderIndex = 2,
                    RenderType = GridRenderType.Text,
                    Width = "130px",
                    Alignment = "center",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = true
                },
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "ProductName",
                    HeaderName = "Ürün Adı",
                    OrderIndex = 3,
                    RenderType = GridRenderType.Text,
                    Width = "auto",
                    Alignment = "left",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = true
                },
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "Brand",
                    HeaderName = "Marka",
                    OrderIndex = 4,
                    RenderType = GridRenderType.Text,
                    Width = "120px",
                    Alignment = "center",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = false
                },
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "StockStatusBadge",
                    HeaderName = "Stok Durumu",
                    OrderIndex = 5,
                    RenderType = GridRenderType.StockBadge,
                    Width = "110px",
                    Alignment = "center",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = true
                },
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "Price",
                    HeaderName = "Fiyat",
                    OrderIndex = 6,
                    RenderType = GridRenderType.Price,
                    Width = "130px",
                    Alignment = "right",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = true
                },
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "QuantityAndCartAction",
                    HeaderName = "Hızlı Sipariş",
                    OrderIndex = 7,
                    RenderType = GridRenderType.QuantityInputWithCart,
                    Width = "180px",
                    Alignment = "center",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = true
                },
                new()
                {
                    GridCode = "B2B_PRODUCT_GRID",
                    FieldName = "DetailAction",
                    HeaderName = "Detay",
                    OrderIndex = 8,
                    RenderType = GridRenderType.ActionModal,
                    Width = "80px",
                    Alignment = "center",
                    IsVisibleDesktop = true,
                    IsVisibleTablet = true,
                    IsVisibleMobile = true
                }
            };

            await context.GridColumnDefinitions.AddRangeAsync(gridColumns);
            await context.SaveChangesAsync();
        }

        // 3. Kullanıcıları Tohumla (Admin ve Bayi)
        if (!await context.Users.AnyAsync())
        {
            // Admin kullanıcısı (admin / Admin123!)
            passwordHasher.CreatePasswordHash("Admin123!", out string adminHash, out string adminSalt);
            var adminUser = new User
            {
                FirstName = "Sistem",
                LastName = "Yöneticisi",
                Email = "admin@c1soft.com",
                Username = "admin",
                PhoneNumber = "05550000001",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                Role = UserRole.Admin,
                IsActive = true
            };

            // Bayi kullanıcısı (bayi1 / Bayi123!)
            passwordHasher.CreatePasswordHash("Bayi123!", out string customerHash, out string customerSalt);
            var customerUser = new User
            {
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                Email = "bayi1@c1soft.com",
                Username = "bayi1",
                PhoneNumber = "05550000002",
                PasswordHash = customerHash,
                PasswordSalt = customerSalt,
                Role = UserRole.Customer,
                IsActive = true
            };

            await context.Users.AddRangeAsync(adminUser, customerUser);
            await context.SaveChangesAsync();
        }

        // 4. Kategorileri Tohumla
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "El Aletleri & Hırdavat", Description = "Profesyonel mekanik ve elektrikli el aletleri" },
                new() { Name = "Elektrik & Aydınlatma", Description = "Endüstriyel kablo, şalter, armatür ve aydınlatma ekipmanları" },
                new() { Name = "Endüstriyel Otomasyon", Description = "Sensörler, PLC üniteleri, motor sürücüler ve röleler" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // 5. Örnek B2B Ürünlerini Tohumla
        if (!await context.Products.AnyAsync())
        {
            var catHirdavat = await context.Categories.FirstAsync(c => c.Name.Contains("Hırdavat"));
            var catElektrik = await context.Categories.FirstAsync(c => c.Name.Contains("Elektrik"));
            var catOtomasyon = await context.Categories.FirstAsync(c => c.Name.Contains("Otomasyon"));

            var products = new List<Product>
            {
                // Bol Stoklu Ürünler (Var - Yeşil)
                new()
                {
                    ProductCode = "HRD-1001",
                    ProductName = "Bosch GSB 18V-50 Kömürsüz Akülü Darbeli Matkap",
                    Description = "Ağır hizmet tipi metal mandrenli 18V profesyonel darbe mekanizmalı akülü delme vidalama.",
                    Brand = "Bosch",
                    ManufacturerCode = "06019H5100",
                    CustomCode1 = "EL-ALET-01",
                    CustomCode2 = "OZEL-HRD-A",
                    ImageUrl = "/images/products/bosch-gsb-18v-50-professional-akusuz-solo-darbeli.avif",
                    StockQuantity = 45,
                    CriticalStockThreshold = 10,
                    Price = 5450.00m,
                    CategoryId = catHirdavat.Id
                },
                new()
                {
                    ProductCode = "ELK-2001",
                    ProductName = "Schneider Acti9 3P 25A 6kA Otomatik Sigorta",
                    Description = "B2B pano tesisatları için C eğrisi minyatür devre kesici koruma ünitesi.",
                    Brand = "Schneider Electric",
                    ManufacturerCode = "A9F74325",
                    CustomCode1 = "PANO-SIG-03",
                    CustomCode2 = "OZEL-ELK-B",
                    ImageUrl = "/images/products/Schneider Acti9 3P 25A 6kA Otomatik Sigorta.jfif",
                    StockQuantity = 120,
                    CriticalStockThreshold = 15,
                    Price = 420.50m,
                    CategoryId = catElektrik.Id
                },
                new()
                {
                    ProductCode = "OTM-3001",
                    ProductName = "Omron E2B-M12KS04-WP-B1 Endüktif Sensör",
                    Description = "M12 silindirik gövde, 4mm algılama mesafesi, PNP NO çıkışlı pirinç sensör.",
                    Brand = "Omron",
                    ManufacturerCode = "E2BM12KS04WPB1",
                    CustomCode1 = "SENSOR-IND-01",
                    CustomCode2 = "OZEL-OTM-C",
                    ImageUrl = "/images/products/Omron E2B-M12KS04-WP-B1 Endüktif Sensör.jfif",
                    StockQuantity = 35,
                    CriticalStockThreshold = 8,
                    Price = 780.00m,
                    CategoryId = catOtomasyon.Id
                },

                // Kritik Stoklu Ürünler (Kritik - Sarı - Stok <= KritikEşik)
                new()
                {
                    ProductCode = "HRD-1002",
                    ProductName = "Makita DGA504Z 18V 125mm Akülü Avuç Taşlama",
                    Description = "Otomatik tork tahrik teknolojisi ve aşırı yük korumalı taşlama makinesi.",
                    Brand = "Makita",
                    ManufacturerCode = "DGA504Z-OEM",
                    CustomCode1 = "EL-ALET-02",
                    CustomCode2 = "OZEL-HRD-B",
                    ImageUrl = "/images/products/Makita DGA504Z 18V 125mm Akülü Avuç Taşlama.jfif",
                    StockQuantity = 4, // Eşiğin altında (Kritik)
                    CriticalStockThreshold = 10,
                    Price = 4850.00m,
                    CategoryId = catHirdavat.Id
                },
                new()
                {
                    ProductCode = "OTM-3002",
                    ProductName = "Siemens SIMATIC S7-1200 CPU 1214C DC/DC/DC",
                    Description = "14 DI / 10 DO / 2 AI entegre PROFINET portlu kompakt PLC kontrol ünitesi.",
                    Brand = "Siemens",
                    ManufacturerCode = "6ES7214-1AG40-0XB0",
                    CustomCode1 = "PLC-CPU-04",
                    CustomCode2 = "OZEL-OTM-D",
                    ImageUrl = "/images/products/Siemens SIMATIC S7-1200 CPU 1214C.jfif",
                    StockQuantity = 2, // Eşiğin altında (Kritik)
                    CriticalStockThreshold = 5,
                    Price = 14200.00m,
                    CategoryId = catOtomasyon.Id
                },

                // Tükendi (Yok - Kırmızı - Stok <= 0)
                new()
                {
                    ProductCode = "ELK-2002",
                    ProductName = "Philips Endüstriyel HighBay LED Armatür 150W 6500K",
                    Description = "Fabrika ve yüksek tavan depo aydınlatması için IP65 dayanımlı projektör armatür.",
                    Brand = "Philips",
                    ManufacturerCode = "BY239P-150W",
                    CustomCode1 = "AYD-HIGH-01",
                    CustomCode2 = "OZEL-ELK-E",
                    ImageUrl = "/images/products/Philips Endüstriyel HighBay LED Armatür 150W 6500K.jfif",
                    StockQuantity = 0, // Sıfır stok (Yok)
                    CriticalStockThreshold = 5,
                    Price = 2890.00m,
                    CategoryId = catElektrik.Id
                },
                new()
                {
                    ProductCode = "HRD-1003",
                    ProductName = "Knipex 00 20 09 V01 B2B Profesyonel Pense Seti 3'lü",
                    Description = "Kombine pense, yan keski ve kargaburun içeren Alman üretim takım seti.",
                    Brand = "Knipex",
                    ManufacturerCode = "002009V01",
                    CustomCode1 = "TAKIM-SET-01",
                    CustomCode2 = "OZEL-HRD-F",
                    ImageUrl = "/images/products/Knipex 00 20 09 V01 B2B Profesyonel Pense Seti 3'lü.jfif",
                    StockQuantity = 18,
                    CriticalStockThreshold = 5,
                    Price = 3150.00m,
                    CategoryId = catHirdavat.Id
                },
                new()
                {
                    ProductCode = "ELK-2003",
                    ProductName = "HES Kablo 3x2.5 NYM Antigron Kablo (100 Metre)",
                    Description = "Bakır iletkenli nemli yer bina ve atölye tesisat kablosu rulosu.",
                    Brand = "HES Kablo",
                    ManufacturerCode = "NYM-3X2.5-100M",
                    CustomCode1 = "KABLO-NYM-01",
                    CustomCode2 = "OZEL-ELK-K",
                    ImageUrl = "/images/products/HES Kablo 3x2.5 NYM Antigron Kablo (100 Metre).jfif",
                    StockQuantity = 60,
                    CriticalStockThreshold = 10,
                    Price = 3650.00m,
                    CategoryId = catElektrik.Id
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
        else
        {
            // Mevcut veritabanı varsa ve eski placeholder görsel yolları kalmışsa gerçek ürün görsellerine eşitle
            var existingProducts = await context.Products.IgnoreQueryFilters().ToListAsync();
            bool hasImageUpdates = false;

            foreach (var p in existingProducts)
            {
                var targetImg = p.ProductCode switch
                {
                    "HRD-1001" => "/images/products/bosch-gsb-18v-50-professional-akusuz-solo-darbeli.avif",
                    "ELK-2001" => "/images/products/Schneider Acti9 3P 25A 6kA Otomatik Sigorta.jfif",
                    "OTM-3001" => "/images/products/Omron E2B-M12KS04-WP-B1 Endüktif Sensör.jfif",
                    "HRD-1002" => "/images/products/Makita DGA504Z 18V 125mm Akülü Avuç Taşlama.jfif",
                    "OTM-3002" => "/images/products/Siemens SIMATIC S7-1200 CPU 1214C.jfif",
                    "ELK-2002" => "/images/products/Philips Endüstriyel HighBay LED Armatür 150W 6500K.jfif",
                    "HRD-1003" => "/images/products/Knipex 00 20 09 V01 B2B Profesyonel Pense Seti 3'lü.jfif",
                    "ELK-2003" => "/images/products/HES Kablo 3x2.5 NYM Antigron Kablo (100 Metre).jfif",
                    _ => null
                };

                if (targetImg != null && p.ImageUrl != targetImg)
                {
                    p.ImageUrl = targetImg;
                    hasImageUpdates = true;
                }
            }

            if (hasImageUpdates)
            {
                await context.SaveChangesAsync();
            }
        }

        // 6. Ana Sayfa Slider / Banner İçeriklerini Tohumla (Madde 4.1)
        if (!await context.Banners.AnyAsync())
        {
            var banners = new List<Banner>
            {
                new()
                {
                    Title = "2026 Endüstriyel Otomasyon & El Aletleri B2B Bahar Kampanyası",
                    Subtitle = "Tüm Bosch ve Siemens ürünlerinde bayilere özel peşin alım avantajları ve anlık teslimat!",
                    ImageUrl = "/images/banners/banner1.jpg",
                    RedirectUrl = "/Product/Index",
                    OrderIndex = 1,
                    IsActive = true
                },
                new()
                {
                    Title = "Schneider & Philips Proje Ürünlerinde Stoktan Hızlı Teslim",
                    Subtitle = "Pano malzemeleri ve yüksek tavan aydınlatma armatürlerinde toptan sipariş fırsatları.",
                    ImageUrl = "/images/banners/banner2.jpg",
                    RedirectUrl = "/Product/Index",
                    OrderIndex = 2,
                    IsActive = true
                }
            };

            await context.Banners.AddRangeAsync(banners);
            await context.SaveChangesAsync();
        }
    }
}
