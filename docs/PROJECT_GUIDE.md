# C1Soft Mini B2B E-Ticaret Projesi - Kesin Teknik Şartname & Mimari Kılavuz
> **DOKÜMAN DURUMU:** Nihai Şartname (Single Source of Truth)  
> **AMACI:** Bu doküman, projede yer alacak tüm katmanları, veritabanı tablolarını, alan tiplerini, iş kurallarını, asgari gereksinimleri ve değerlendirme kriterlerini **sıfır muğlaklıkla (net ve kesin olarak)** tanımlar. Her geliştirici ve yapay zeka ajanı bu kurallara harfiyen uymakla yükümlüdür.

---

## BÖLÜM 1: TEKNOLOJİ YIĞINI & ÇALIŞMA ORTAMI (STACK)

| Bileşen | Seçilen Teknoloji / Standart | Açıklama |
| :--- | :--- | :--- |
| **Platform** | .NET 8 / 10 - C# (Modern LTS) | ASP.NET Core MVC |
| **Veritabanı Motoru** | **Microsoft SQL Server (MS SQL)** | `(localdb)\MSSQLLocalDB` (Windows / Visual Studio yerel motoru) |
| **Bağlantı Dizesi (Connection String)** | `Server=(localdb)\MSSQLLocalDB;Database=C1SoftB2BDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;` | Dış sunucu / Docker gerektirmeden tek tıkla çalışır |
| **ORM / Veri Erişimi** | Entity Framework Core (Code-First) + Optimized LINQ/SQL | SQL seviyesinde filtreleme ve projeksiyon |
| **Validasyon Kütüphanesi** | FluentValidation | Kurallar Entity içinde değil, ayrı validator sınıflarında |
| **Kimlik Doğrulama (Auth)** | Cookie Authentication | Şifreler PBKDF2 / SHA256 Salted Hash ile saklanır (Açık metin YASAK) |
| **Yetkilendirme (Authz)** | Role-Based Authorization | `Admin` ve `Customer` (Bayi) rolleri |
| **Ön Yüz Kütüphaneleri** | Bootstrap 5, Vanilla JS (Fetch API), SweetAlert2 | Temiz, kurumsal B2B arayüzü |

---

## BÖLÜM 2: VERİTABANI ŞEMASI VE ASGARİ ALAN LİSTESİ (DATA DICTIONARY)

Case'de istenen asgari gereksinimler ve B2B için eklenen kritik kolonların veri tipleri:

### 2.1. `Products` (Ürünler Tablosu)
* `Id` (int, PK, Identity)
* `ProductCode` (nvarchar(50), Unique, Not Null) - **Ürün Kodu**
* `ProductName` (nvarchar(200), Not Null) - **Ürün Adı**
* `Description` (nvarchar(max), Nullable) - **Açıklama**
* `Brand` (nvarchar(100), Not Null) - **Marka**
* `ManufacturerCode` (nvarchar(100), Nullable) - **Üretici Kodu**
* `CustomCode1` (nvarchar(100), Nullable) - **Özel Kod 1**
* `CustomCode2` (nvarchar(100), Nullable) - **Özel Kod 2**
* `ImageUrl` (nvarchar(500), Nullable) - **Resim Yolu** (Varsayılan placeholder atanır)
* `StockQuantity` (int, Not Null, >= 0) - **Stok Miktarı**
* `CriticalStockThreshold` (int, Not Null, Default: 10) - **Ürün Bazlı Kritik Stok Eşiği** (Madde 6.1)
* `Price` (decimal(18,2), Not Null, > 0) - **Fiyat**
* `CategoryId` (int, FK -> Categories.Id, Not Null) - **Kategori İlişkisi**
* `IsActive` (bit, Not Null, Default: 1)
* `CreatedAt` (datetime2, Not Null, Default: GETDATE())

### 2.2. `Categories` (Kategoriler Tablosu)
* `Id` (int, PK, Identity)
* `Name` (nvarchar(100), Not Null)
* `Description` (nvarchar(250), Nullable)

### 2.3. `Users` (Kullanıcılar Tablosu)
* `Id` (int, PK, Identity)
* `FirstName` (nvarchar(50), Not Null) - **Ad**
* `LastName` (nvarchar(50), Not Null) - **Soyad**
* `Email` (nvarchar(100), Unique, Not Null) - **E-posta**
* `Username` (nvarchar(100), Unique, Not Null) - **Kullanıcı Adı**
* `PhoneNumber` (nvarchar(20), Nullable) - **Telefon**
* `PasswordHash` (nvarchar(256), Not Null) - **Güvenli Şifre Özeti** (Hash)
* `PasswordSalt` (nvarchar(256), Not Null) - **Şifre Tuzu** (Salt)
* `Role` (nvarchar(20), Not Null, "Admin" veya "Customer") - **Yetki Rolü**
* `IsActive` (bit, Not Null, Default: 1)
* `CreatedAt` (datetime2, Not Null)

### 2.4. `Carts` ve `CartItems` (Sepet Tabloları)
* **`Carts`**:
  * `Id` (int, PK, Identity)
  * `UserId` (int, FK -> Users.Id, Unique, Not Null) - Her bayinin 1 aktif sepeti bulunur
  * `UpdatedAt` (datetime2, Not Null)
* **`CartItems`**:
  * `Id` (int, PK, Identity)
  * `CartId` (int, FK -> Carts.Id, Not Null)
  * `ProductId` (int, FK -> Products.Id, Not Null)
  * `Quantity` (int, Not Null, > 0)

### 2.5. `Orders` ve `OrderItems` (Sipariş ve Fiyat Snapshot Tabloları)
* **`Orders`**:
  * `Id` (int, PK, Identity)
  * `OrderNumber` (nvarchar(30), Unique, Not Null) - Örn: `ORD-20260922-0001`
  * `UserId` (int, FK -> Users.Id, Not Null)
  * `OrderDate` (datetime2, Not Null)
  * `Status` (nvarchar(20), Not Null) - En az `Pending` (Beklemede), `Approved` (Onaylandı), `Rejected` (Reddedildi)
  * `TotalAmount` (decimal(18,2), Not Null)
  * `AdminNote` (nvarchar(500), Nullable) - Red veya onay açıklaması
* **`OrderItems` (Snapshot Koruma Tablosu - Madde 9)**:
  * `Id` (int, PK, Identity)
  * `OrderId` (int, FK -> Orders.Id, Not Null)
  * `ProductId` (int, FK -> Products.Id, Not Null)
  * `ProductCode` (nvarchar(50), Not Null) - **Sipariş anındaki ürün kodu**
  * `ProductName` (nvarchar(200), Not Null) - **Sipariş anındaki ürün adı**
  * `UnitPrice` (decimal(18,2), Not Null) - **Sipariş anındaki birim fiyat**
  * `Quantity` (int, Not Null, > 0) - **Adet**
  * `TotalPrice` (decimal(18,2), Not Null) - `UnitPrice * Quantity`

### 2.6. `GridColumnDefinitions` (Dinamik Grid Konfigürasyonu - Madde 6.1)
* `Id` (int, PK, Identity)
* `GridCode` (nvarchar(50), Not Null, Örn: "B2B_PRODUCT_GRID")
* `FieldName` (nvarchar(50), Not Null) - Modellerdeki property adı (`ImageUrl`, `ProductCode`, `ProductName`, `Brand`, `StockStatusBadge`, `Price`, `QuantityAndCartAction`, `DetailAction`)
* `HeaderName` (nvarchar(100), Not Null) - Sütun başlığı ("Resim", "Ürün Kodu", "Adı", "Marka", "Stok", "Fiyat", "Sipariş Adedi")
* `OrderIndex` (int, Not Null) - Görüntülenme sırası (1, 2, 3...)
* `RenderType` (nvarchar(50), Not Null) - `Image`, `Text`, `Badge`, `Price`, `QuantityInputWithCart`, `ActionModal`
* `Width` (nvarchar(20), Nullable) - Örn: "80px", "150px", "auto"
* `Alignment` (nvarchar(10), Not Null, Default: "center") - "left", "center", "right"
* `IsVisibleDesktop` (bit, Not Null, Default: 1)
* `IsVisibleTablet` (bit, Not Null, Default: 1)
* `IsVisibleMobile` (bit, Not Null, Default: 1)

### 2.7. `Banners` (Ana Sayfa Slider İçerikleri - Madde 4.1)
* `Id` (int, PK, Identity)
* `Title` (nvarchar(150), Not Null)
* `Subtitle` (nvarchar(250), Nullable)
* `ImageUrl` (nvarchar(500), Not Null)
* `RedirectUrl` (nvarchar(500), Nullable)
* `OrderIndex` (int, Not Null, Default: 0)
* `IsActive` (bit, Not Null, Default: 1)

---

## BÖLÜM 3: CASE ASGARİ GEREKSİNİMLERİ VE İŞ KURALLARI (BUSINESS LOGIC)

### 3.1. Stok Gösterge Kuralı (B2B Grid)
Sabit bir eşik YOKTUR; her ürünün kendi `CriticalStockThreshold` değeri üzerinden hesaplanır:
* **`StockQuantity > CriticalStockThreshold`** ➔ **Yeşil Rozet: "Var"**
* **`0 < StockQuantity <= CriticalStockThreshold`** ➔ **Sarı Rozet: "Kritik"**
* **`StockQuantity <= 0`** ➔ **Kırmızı Rozet: "Yok"**

### 3.2. Doğrudan Tablodan Sipariş Girişi
* Bayi, ürün detayına girmeden ürün satırındaki sayı kutusuna adedi yazıp "Sepete Ekle" butonuna basabilir.
* Sistem arkaplanda anlık Fetch isteği ile ürünü sepete ekler ve kullanıcıya başarı bildirimi (SweetAlert/Toast) gösterir.

### 3.3. Ürün Detay Popup (Modal)
* Listeden herhangi bir ürünün detayına tıklandığında sayfa yenilenmeden **Popup (Modal)** açılır.
* Modal içinde: Resim, Ürün Kodu, Adı, Açıklaması, Markası, Üretici Kodu, Özel Kod 1 & 2, Stok Durumu, Fiyatı gösterilir.

### 3.4. SQL Seviyesinde Gelişmiş Metinsel Arama (Madde 6 & 12)
* Sayısal kolonlar hariç metinsel olan **tüm** alanlar aranabilir olmalıdır:
  * `ProductName LIKE @query` OR `ProductCode LIKE @query` OR `Brand LIKE @query` OR `ManufacturerCode LIKE @query` OR `Description LIKE @query` OR `CustomCode1 LIKE @query` OR `CustomCode2 LIKE @query`
* **Kesin Kural:** Veriler belleğe çekilerek C# içinde filtrelenemez (`context.Products.ToList().Where(...)` kesinlikle yasaktır). Filtreleme `IQueryable` üzerinden SQL Server'a `WHERE` cümlesi olarak gönderilmelidir.

### 3.5. Sipariş Doğrulama, Atomik DB Transaction ve Snapshot (Madde 8 & 9)
"Sipariş Oluştur" butonuna tıklandığında:
1. **Frontend + Backend Çift Kontrol:** Sadece JavaScript kontrolü yetersizdir; backend'de sipariş anında sepetteki ürünlerin stokları tek tek kilitlenip okunur.
2. **Yetersiz Stok Durumu:** Sepetteki herhangi bir ürün için `TalepAdedi > StockQuantity` ise işlem derhal durdurulur ve açıkça:
   `"Ürün {ProductName} için yeterli stok bulunmamaktadır. Mevcut stok: {StockQuantity}."` mesajı dönülür.
3. **Transaction ve Stok Düşümü:** Stoklar yeterliyse `using var transaction = await context.Database.BeginTransactionAsync();` başlatılır:
   * Her ürünün `StockQuantity` değeri sipariş adedi kadar azaltılır.
   * `Orders` kaydı oluşturulur.
   * `OrderItems` kaydı oluşturulurken ürünün **o anki adı, kodu ve birim fiyatı** kopyalanır.
   * Sepet temizlenir (`CartItems` silinir).
   * `await transaction.CommitAsync();` yapılarak işlem atomik olarak tamamlanır.

### 3.6. Sipariş Durum Yönetimi ve Bayi Ekranına Anlık Yansıma (Madde 3 & 10)
* Yönetici Admin Panelinden siparişin durumunu `Onaylandı` veya `Reddedildi` yapabilir.
* Bayi "Siparişlerim" ekranına girdiğinde güncel durumu ve sipariş detayındaki snapshot fiyatları anında görür.

---

## BÖLÜM 4: TEST HESAPLARI VE SEED DATA (İLK KURULUM BİLGİLERİ)

Veritabanı oluşturulduğunda otomatik tohumlanacak (seed edilecek) varsayılan veriler:
1. **Yönetici Hesabı (Admin):**
   * Kullanıcı Adı: `admin`
   * E-posta: `admin@c1soft.com`
   * Şifre: `Admin123!` (Hashlenmiş olarak saklanacaktır)
   * Rol: `Admin`
2. **Bayi Hesabı (Müşteri):**
   * Kullanıcı Adı: `bayi1`
   * E-posta: `bayi1@c1soft.com`
   * Şifre: `Bayi123!` (Hashlenmiş olarak saklanacaktır)
   * Rol: `Customer`
3. **Örnek Ürünler:**
   * En az 8-10 adet gerçekçi B2B ürünü (farklı markalar, özel kodlar, stok seviyeleri - bazıları bol stoklu, bazıları kritik stoklu, bazıları tükenmiş).
4. **Dinamik Grid Kolonları:**
   * Tablo için 7 temel kolon tanımı.
5. **Örnek Bannerlar:**
   * Ana sayfa kampanya slider görselleri.

---

## BÖLÜM 5: PROJE TESLİMAT KONTROL LİSTESİ (CHECKLIST)

- [x] **1. Kaynak Kod:** Temiz N-Tier mimari, 0 hata, 0 uyarı.
- [x] **2. Veritabanı Oluşturma:** EF Core Migrations + Proje ana dizininde `Database_Script.sql` çıktısı.
- [ ] **3. README.md:** Kurulum adımları, Connection String, varsayılan kullanıcılar ve mimari tercihler.
- [x] **4. SQL Bilgisi:** İlişkisel tablolar, Foreign Key'ler, Unique kısıtlamalar, gereksiz veri çekmeyen SQL projeksiyonları.
- [x] **5. Güvenlik:** PBKDF2 şifre hashleme, Cookie Auth, Rol bazlı yetkilendirme.
