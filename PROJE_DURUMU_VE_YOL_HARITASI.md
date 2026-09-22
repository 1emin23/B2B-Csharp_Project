# C1Soft B2B E-Ticaret Platformu - Proje Durum Raporu ve Yol Haritası

> **Proje Niteliği:** Bu proje bir teknik değerlendirme case'idir. Aşırı karmaşık, over-engineered kurumsal kalıplar yerine; **sıfır mantık hatası (zero logic bug)**, **tam veri tutarlılığı**, **sağlam backend mimarisi** ve **temiz/fonksiyonel bir kullanıcı arayüzü** hedeflenmektedir.

---

## 1. PROJE GENEL BAKIŞI VE MİMARİ YAPI

Proje, **.NET 10 & ASP.NET Core MVC** üzerinde N-Tier (Çok Katmanlı) mimari prensiplerine göre yapılandırılmıştır:

1. **`C1Soft.Domain` (Çekirdek Varlık Katmanı):**
   - Bağımsız temel entity'ler, ilişkisel veritabanı modelleri ve enum yapıları.
2. **`C1Soft.DataAccess` (Veri Erişim Katmanı):**
   - Entity Framework Core, Code-First Fluent API konfigürasyonları, `AppDbContext`, PBKDF2/SHA256 salted şifreleme motoru, `DbInitializer` (Seed Data) ve veritabanı scriptleri.
3. **`C1Soft.Business` (İş Mantığı Katmanı):**
   - DTO modelleri, FluentValidation doğrulama kuralları, SQL seviyesinde IQueryable arama, atomik transaction yönetimi ve servis sınıfları.
4. **`C1Soft.WebUI` (Sunum Katmanı):**
   - MVC Controller'ları, Cookie Authentication, dinamik grid arayüzü, AJAX/Fetch bazlı sepet etkileşimleri ve Bootstrap 5 arayüzü.

---

## 2. ŞİMDİYE KADAR YAPILANLAR (TAMAMLANAN İŞLER)

### ✅ 2.1. Domain & Varlık Katmanı
- [x] **`Product` Entity:** Ürün kodu, ürün adı, marka, üretici kodu, özel kod 1 & 2, resim yolu, stok miktarı, ürün bazlı dinamik kritik stok eşiği (`CriticalStockThreshold`), fiyat ve kategori ilişkisi tanımlandı.
- [x] **`Category` Entity:** Kategori adı ve açıklama alanları tanımlandı.
- [x] **`User` Entity:** PBKDF2 şifre hash ve salt alanları, ad/soyad, e-posta, kullanıcı adı, telefon ve `UserRole` (`Admin`, `Customer`) tanımlandı.
- [x] **`Cart` & `CartItem` Entity:** Her bayiye özel tekil sepet ve ilişkili sepet kalemleri tanımlandı.
- [x] **`Order` & `OrderItem` (Snapshot Koruma) Entity:** Sipariş ana tablosu ve fiyat manipülasyonunu engelleyen sipariş anındaki ürün adı, ürün kodu ve birim fiyatı donduran snapshot kalemleri oluşturuldu.
- [x] **`GridColumnDefinition` Entity:** B2B tablosunun kolon sırasını, başlığını, hizalamasını, genişliğini ve görünürlüğünü veritabanından yöneten dinamik grid modeli oluşturuldu.
- [x] **`Banner` Entity:** Ana sayfa slider ve kampanya içerikleri için model hazırlandı.

### ✅ 2.2. DataAccess & Veritabanı Katmanı
- [x] **Fluent API Konfigürasyonları:** Tablo isimleri, foreign key ilişkileri, cascade/restrict silme kuralları, unique index kısıtları (`ProductCode`, `Email`, `Username`, `OrderNumber`) tanımlandı.
- [x] **Global Query Filter:** Pasif (`IsActive = false`) ürün ve kullanıcıların bayi sorgularından otomatik gizlenmesi, admin tarafında `.IgnoreQueryFilters()` ile yönetilmesi sağlandı.
- [x] **Şifreleme Motoru (`PasswordHasher`):** Düz metin şifre saklama engellendi. PBKDF2 / SHA256 ile tuzlanmış (salted) güvenli hash mekanizması yazıldı.
- [x] **Veritabanı Tohumlama (`DbInitializer`):**
  - Yönetici hesabı (`admin` / `Admin123!`)
  - Bayi hesabı (`bayi1` / `Bayi123!`)
  - 3 kategori
  - 8 adet gerçekçi B2B ürünü (bol stoklu, kritik stoklu, tükenmiş stok senaryoları)
  - 8 adet dinamik grid kolon tanımı
  - 2 adet kampanya banner'ı
- [x] **Veritabanı Migration ve SQL Çıktısı:** `InitialCreate` migration'ı oluşturuldu ve `Database_Script.sql` teslimata hazır hale getirildi.

### ✅ 2.3. Business & İş Kuralları Katmanı
- [x] **DTO Yapısı:** Auth, Product, Cart, Order, User işlemleri için sunum katmanına özel DTO sınıfları yazıldı.
- [x] **FluentValidation Kuralları:** `LoginValidator`, `RegisterValidator`, `ProductValidator` sınıfları yazılarak giriş doğrulamaları entity bağımsız hale getirildi.
- [x] **SQL Düzeyinde Metinsel Arama (`ProductService.GetProductGridAsync`):** Belleğe çekmeden (`ToList().Where()` KULLANILMADAN), doğrudan SQL Server'a `EF.Functions.Like` gönderen arama motoru tamamlandı (`ProductName`, `ProductCode`, `Brand`, `ManufacturerCode`, `Description`, `CustomCode1`, `CustomCode2`).
- [x] **Sepet Mantığı (`CartService`):**
  - Sepete ekleme, adet artırma/azaltma, silme, boşaltma ve sepet sayısı sayaç metodları yazıldı.
  - Sepete eklerken ürünün aktifliği ve toplam adet stok aşımı kontrolü yapıldı.
- [x] **Atomik Sipariş & Stok Yönetimi (`OrderService.CreateOrderAsync`):**
  - `Database.BeginTransactionAsync` ile çift stok kontrolü sağlandı.
  - Sipariş oluşturulurken ürün stokları düşürüldü.
  - Snapshot kalemleri oluşturuldu ve sepet temizlendi.
- [x] **Sipariş Durum Güncelleme:** Admin için `Pending`, `Approved`, `Rejected` durum geçişi ve yönetici notu alanı sağlandı.
- [x] **Kullanıcı & Kimlik Yönetimi (`AuthService`, `UserService`):** Login, Register, ClaimsPrincipal üretimi, bayi listeleme ve düzenleme servisleri yazıldı.
- [x] **Derleme Durumu:** Çözüm 0 hata ile başarıyla derlenmektedir (`dotnet build` PASS).

---

## 3. CASE'DE İSTENEN KRİTİK İŞ KURALLARI VE EDGE CASE'LER (ÖNEMLİ)

Case değerlendirmesinde özellikle dikkat edilecek ve arkasında logic hatası bırakılmayacak noktalar:

1. **Dinamik Stok Rozeti Hesaplaması (Madde 3.1 & 6.1):**
   - *Kural:* Sabit bir stok eşiği (örn: 10) TÜM ürünler için geçerli değildir! Her ürünün kendi `CriticalStockThreshold` değeri vardır.
   - *Mantık:*
     - `StockQuantity > CriticalStockThreshold` ➔ **Yeşil ("Var")**
     - `0 < StockQuantity <= CriticalStockThreshold` ➔ **Sarı ("Kritik")**
     - `StockQuantity <= 0` ➔ **Kırmızı ("Yok")**
   - *Edge Case:* Stok 0 ise miktar kutusu ve "Sepete Ekle" butonu deaktif olmalı; kullanıcı elle tarayıcıdan istek atsa dahi backend servisi `ArgumentException` fırlatıp işlemi reddetmeli.

2. **Sepete Ekleme ve Miktar Aşımı Edge Case'i:**
   - *Mantık:* Ürünün stoğu 5 adet. Kullanıcı önce 3 adet ekledi. Sonra tekrar 3 adet daha eklemek istedi.
   - *Backend Kontrolü:* Sadece yeni eklenen adede değil, `(Sepetteki Mevcut Adet + Yeni Adet) > Mevcut Stok` formülüne bakılır. Aşım varsa açık hata mesajı fırlatılır: `"Sepete eklemek istediğiniz toplam adet (6), mevcut stoktan (5) fazla."`

3. **Sipariş Onayında Yarış Durumu (Race Condition) & Çift Kontrol (Madde 3.5 & 8):**
   - *Problem:* Bayi ürünü sepete attığında stok vardı. Ancak sipariş butonuna basana kadar başka bir bayi ürünü tüketti.
   - *Çözüm:* Sadece sepete atarken değil, siparişi tamamlama butonuna basıldığı an `OrderService` veritabanından en güncel stoğu okur (`item.Quantity > product.StockQuantity`). Yetersizse transaction iptal edilir (`Rollback`), sepet silinmez ve bayiye hangi ürünün bittiği açıkça iletilir.

4. **Fiyat ve Ürün Snapshot Koruma (Madde 9):**
   - *Problem:* Bayi 100 TL'den sipariş verdi. 2 gün sonra admin ürünün adını değiştirdi veya fiyatını 300 TL yaptı.
   - *Çözüm:* `OrderItems` tablosunda `ProductId`'nin yanında sipariş anındaki `UnitPrice`, `ProductName` ve `ProductCode` ayrı kolonlarda kalıcı dondurulur. Geçmiş siparişler asla ana ürün tablosundaki fiyat güncellemelerinden etkilenmez.

5. **SQL Seviyesinde Gelişmiş Arama Performansı (Madde 3.4 & 12):**
   - *Problem:* Binlerce ürün olduğunda `context.Products.ToList().Where(...)` yazmak sunucu RAM'ini tüketir ve case değerlendirmesinde eksi puan getirir.
   - *Çözüm:* Filtreleme `IQueryable` zinciri üzerinden SQL Server'a `WHERE ... LIKE %query%` parametresiyle gider.

6. **Yetkilendirme ve Veri İzolasyonu:**
   - Bayi yalnızca kendi siparişlerini ve sepetini görebilir (`UserId` filtresi).
   - Admin ekranlarına normal kullanıcı giremez (Role: `Admin` kısıtlaması).
   - Pasif edilen ürünler ve kullanıcılar sistemde işlem yapamaz.

---

## 4. BUNDAN SONRA YAPILACAKLAR (ADIM ADIM VE KESİN DİLLE)

Aşağıdaki adımlar, case'in doğasına uygun şekilde; gereksiz süslemelerden uzak, temiz Bootstrap 5 ve çalışan işlevsel backend uçlarıyla sırasıyla hayata geçirilecektir:

### ✅ ADIM 1: Kimlik Doğrulama Ekranları (Auth Web Flow) - [TAMAMLANDI]
- **Yapılanlar:**
  - `AuthController.cs` oluşturuldu: `Login` (GET/POST), `Register` (GET/POST), `Logout` (POST), `AccessDenied` (GET).
  - Cookie Authentication `SignInAsync` entegrasyonu tamamlandı; Admin `/Admin/Orders`, Bayi ise `/Product/Index` sayfasına otomatik yönlendirildi.
  - `Views/Auth/Login.cshtml` ve `Views/Auth/Register.cshtml` temiz Bootstrap 5 ile oluşturuldu; inceleyen kişinin anında test edebilmesi için Admin ve Bayi test hesapları bilgi kartı eklendi.
  - `Views/Shared/_Layout.cshtml` modern B2B düzenine kavuşturuldu (Dinamik Auth dropdown, sepet rozeti, Bootstrap Icons, SweetAlert2).


### ✅ ADIM 2: B2B Dinamik Ürün Grid Ekranı & Hızlı Sipariş (Bayi Kataloğu) - [TAMAMLANDI]
- **Yapılanlar:**
  - `ProductIndexViewModel.cs` ve `ProductController.cs` oluşturuldu (`Index` ve `Detail` aksiyonları).
  - `Views/Product/Index.cshtml` geliştirildi:
    - SQL seviyesinde IQueryable arama formu (`search` parametresiyle tüm metinsel alanlarda LIKE filtreleme).
    - `GridColumnDefinitions` tablosuna bağlı dinamik kolon render motoru (OrderIndex, genişlik, hizalama, responsive görünürlük).
    - Ürün bazlı kritik stok eşiğine göre dinamik rozetler (`Var`, `Kritik`, `Yok`).
    - Tablo üzerinden anında adet girip sepete ekleme input & buton grubu (Stokta olmayanlarda otomatik deaktif).
    - Bootstrap 5 Detay Popup Modalı (Ürün kodu, marka, üretici kodu, özel kodlar, kategori ve açıklamayı sayfa yenilemeden Fetch ile getirme).


### 📌 ADIM 3: AJAX Sepet Yönetimi & Navbar Bildirimi
- **Yapılacaklar:**
  - `CartController.cs` oluşturulacak:
    - `Index()`: Bayinin sepet sayfasını dönecek (`GetCartAsync`).
    - `[HttpPost] AddToCart(int productId, int quantity)`: AJAX ile çağrılacak. Başarılı/hata durumunu JSON dönecek (`{ success: true, message: "...", cartCount: 3 }`).
    - `[HttpPost] UpdateQuantity(int cartItemId, int quantity)`: AJAX ile sepet içinden adet artırıp azaltacak.
    - `[HttpPost] Remove(int cartItemId)`: Kalemi sepetten silecek.
    - `[HttpGet] GetCartCount()`: Navbar'daki sepet sayısını tazeleyecek.
  - `Views/Cart/Index.cshtml`:
    - Sepet tablosu, birim fiyat, adet değiştirme inputları, satır tutarı, sepet genel toplamı ve "Siparişi Onayla" butonu.
  - `Views/Shared/_Layout.cshtml` düzenlenecek:
    - Navbar'da dinamik Sepet Rozeti (Badge), Oturum açan kullanıcının adı ve rolü, "Çıkış Yap" butonu.
    - AJAX yanıtları için SweetAlert2 kütüphanesi entegre edilecek (Sepete eklendiğinde sağ üstte yeşil toast bildirimi).

### 📌 ADIM 4: Sipariş Tamamlama & Snapshot Sipariş Geçmişi
- **Yapılacaklar:**
  - `OrderController.cs` oluşturulacak:
    - `[HttpPost] Checkout()`: Sepetteki ürünlerle `OrderService.CreateOrderAsync` çağıracak. Hata oluşursa (yetersiz stok vb.) SweetAlert ile net hata mesajı gösterecek. Başarılıysa sipariş numarasıyla teşekkür/özet sayfasına yönlendirecek.
    - `MyOrders()`: Bayinin geçmiş sipariş listesini listeleyecek (`GetOrdersByUserAsync`).
    - `Detail(int id)`: Siparişin snapshot kalemlerini, o anki fiyatlarını, durumunu (`Beklemede`, `Onaylandı`, `Reddedildi`) ve varsa yönetici notunu gösterecek.
  - `Views/Order/MyOrders.cshtml` ve `Views/Order/Detail.cshtml` hazırlanacak.

### 📌 ADIM 5: Yönetim (Admin) Paneli
- **Yapılacaklar:**
  - `[Authorize(Roles = "Admin")]` özniteliği ile korunan Admin controller'ları yazılacak:
    - `AdminOrderController.cs`:
      - Tüm siparişleri listeleme (`GetAllOrdersAsync`).
      - Sipariş durumunu değiştirme (`Approved` / `Rejected`) ve yönetici açıklaması (`AdminNote`) girip kaydetme.
    - `AdminProductController.cs`:
      - Ürün listesi (aktif/pasif ayrımıyla).
      - Yeni ürün ekleme ve düzenleme formu (FluentValidation ile korunan).
      - Ürün silme (Soft-delete: `IsActive = false`).
  - Admin için sade Bootstrap tabloları ve modal/form sayfaları hazırlanacak.

### 📌 ADIM 6: Test, Doğrulama ve Teslimat Belgeleri (README.md)
- **Yapılacaklar:**
  - Tüm akışlar uçtan uca test edilecek:
    - Giriş yapma (Admin ve Bayi)
    - SQL arama testi (tüm metinsel alanlarda)
    - Stok eşik rozetleri testi (Var/Kritik/Yok)
    - Hızlı sipariş ve sepet limitleri testi
    - Atomik transaction ve stok düşüm testi
    - Admin sipariş onay/red akışı ve bayiye anlık yansıması
  - Kök dizindeki `README.md` eksiksiz hazırlanacak:
    - Kurulum adımları (`(localdb)\MSSQLLocalDB`)
    - Migration ve Seed Data açıklaması
    - Test kullanıcı bilgileri
    - Mimari kararlar ve case gereksinim karşılama matrisi.
