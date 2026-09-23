# C1Soft — Mini B2B E-Ticaret Platformu

> **Teknik Değerlendirme & Yazılım Geliştirme Case Projesi**  
> Bu proje; bayilerin ürün kataloğunu inceleyip doğrudan tablo üzerinden hızlı sipariş verebildiği, sepet oluşturduğu, atomik stok kontrolü ve fiyat snapshot güvencesiyle sipariş oluşturduğu; yöneticinin ise siparişleri onaylayıp/reddedebildiği, ürünleri ve kullanıcıları yönetebildiği uçtan uca çalışan kurumsal bir **B2B E-Ticaret Web Uygulaması**dır.

---

## 📑 İÇİNDEKİLER

1. [Proje Nedir ve Hangi Süreçleri Kapsar?](#1-proje-nedir-ve-hangi-süreçleri-kapsar)
2. [Kullanılan Teknolojiler (Tech Stack)](#2-kullanılan-teknolojiler-tech-stack)
3. [Mimari Yapı ve Teknik Tercihler](#3-mimari-yapı-ve-teknik-tercihler)
4. [Öne Çıkan Kritik B2B Çözümleri](#4-öne-çıkan-kritik-b2b-çözümleri)
5. [Kurulum ve Çalıştırma Kılavuzu](#5-kurulum-ve-çalıştırma-kılavuzu)
6. [Veritabanı Entegrasyonu ve Script Bilgisi](#6-veritabanı-entegrasyonu-ve-script-bilgisi)
7. [Varsayılan Test Hesapları (Seed Data)](#7-varsayılan-test-hesapları-seed-data)
8. [Case Gereksinimleri Karşılama Matrisi](#8-case-gereksinimleri-karşılama-matrisi)

---

## 1. PROJE NEDİR VE HANGİ SÜREÇLERİ KAPSAR?

Proje, kurumsal B2B toptan ticaret dinamiklerine göre tasarlanmış olup iki ana bölümden oluşmaktadır:

### 1.1. Kullanıcı Arayüzü (Bayi / Frontend)
- **Ana Sayfa:** Veritabanından yönetilen kampanya, duyuru ve reklam amaçlı dinamik **Slider / Banner Carousel** alanı (Madde 4.1).
- **Kimlik Doğrulama:** Cookie tabanlı güvenli giriş (`Login`) ve yeni bayi kayıt (`Register`) ekranları.
- **B2B Dinamik Ürün Grid'i:** Bireysel e-ticaret sitelerindeki hantal kutu (kart) tasarımı yerine, bayilerin yüzlerce ürünü tek ekranda hızlıca tarayabileceği **satır bazlı B2B Tablo Görünümü** (Madde 6.1).
- **Ürün Bazlı Dinamik Kritik Stok Rozetleri:** Sabit bir eşik yerine, her ürünün kendi eşiğine göre hesaplanan renkli göstergeler (*Var - Yeşil*, *Kritik - Sarı*, *Yok - Kırmızı*).
- **Hızlı Sipariş Girişi:** Ürün detayına girmeden, doğrudan satırdaki adet kutusundan tek tıkla sepete ekleme (Madde 3.2 & 6.1).
- **Detay Popup (Modal):** Sayfa yenilenmeden asenkron Fetch API ile açılan ve tüm özel kodları gösteren ürün detay penceresi.
- **AJAX Sepet Yönetimi:** Sepette anlık adet güncelleme, ürün çıkarma, sepeti temizleme ve Navbar sepet sayaç rozeti entegrasyonu.
- **Sipariş Geçmişi & Snapshot Faturası:** Geçmiş siparişlerin durum takibi ve sipariş anındaki dondurulmuş fiyatların incelenmesi.

### 1.2. Yönetim Paneli (Admin Panel)
- **Sipariş Yönetimi:** Tüm siparişlerin listelenmesi, detay faturası, sipariş durumunun **Onaylandı** veya **Reddedildi** olarak güncellenmesi ve bayiye iletilecek **Yönetici Açıklama Notu** girilmesi (Madde 3).
- **Ürün Yönetimi:** Katalogdaki tüm ürünleri filtreleme, yeni ürün ekleme (resim upload, kritik stok eşiği, kategori ve özel kodlar dahil), ürün düzenleme ve ilişkisel bütünlüğü koruyan **Soft-Delete** (pasife alma) mekanizması (Madde 1.1).
- **Slider / Banner Yönetimi (Ekstra Puan - Madde 4.1):** Ana sayfa carousel alanındaki kampanya slider'larının yönetim panelinden dinamik eklenmesi, görsel yükleme/değiştirme, hedef link ve sıra no belirlenmesi, tek tıkla aktif/pasif yapılması ve silinmesi.
- **Bayi & Kullanıcı Yönetimi:** Sisteme kayıtlı kullanıcıların listelenmesi, aranması, rol (`Admin`/`Customer`), hesap aktifliği ve güvenli şifre güncellenmesi (Madde 2).
- **Asilzade & Dark Executive Konsolu (Yönetici Masası):** Admin oturumu açıldığında sayfa tepesinde kurumsal **"👑 YÖNETİCİ KONSOLU"** şeridi, altın sarısı rozetler, yönetim sayfalarında (`/Admin/*`) koyu gece mavisi ve asil altın ERP teması ve Ürün Kataloğunda Admin için "Sepete Ekle" yerine doğrudan satırdan **"Düzenle"** kısayol entegrasyonu.

---

## 2. KULLANILAN TEKNOLOJİLER (TECH STACK)

| Katman / Bileşen | Kullanılan Teknoloji | Açıklama |
| :--- | :--- | :--- |
| **Çalışma Zamanı (Runtime)** | **.NET 10 (LTS)** | ASP.NET Core MVC |
| **Programlama Dili** | **C# 13** | Nullable reference types, Pattern Matching, Records |
| **Veritabanı Motoru** | **Microsoft SQL Server** | `(localdb)\MSSQLLocalDB` (Yerel geliştirme motoru) |
| **ORM / Veri Erişimi** | **Entity Framework Core 10** | Code-First, Fluent API, IQueryable SQL Optimizasyonu |
| **Validasyon Kütüphanesi** | **FluentValidation** | İş kuralları ve DTO seviyesinde doğrulama kuralları |
| **Kimlik Doğrulama (Auth)** | **Cookie Authentication** | ClaimsPrincipal tabanlı oturum yönetimi |
| **Kriptografik Güvenlik** | **PBKDF2 / SHA256** | Tuzlanmış (Salted) güvenli şifre özetleme motoru |
| **Kullanıcı Arayüzü (UI)** | **Bootstrap 5 & Bootstrap Icons** | Responsive, modern B2B portal teması |
| **İstemci Etkileşimleri** | **Vanilla JS (Fetch API) & SweetAlert2** | Sayfa yenilemesiz hızlı sepet aksiyonları ve toast bildirimleri |

---

## 3. MİMARİ YAPI VE TEKNİK TERCİHLER

Proje, **Sorumlulukların Ayrılığı (Separation of Concerns)** prensibine tam uyumlu, bakımı kolay ve ölçeklenebilir **4 Katmanlı N-Tier Mimari** ile geliştirilmiştir:

```
C1Soft/
├── C1Soft.Domain/           ← Katman 1: Entity sınıfları, Enum modelleri (Tamamen bağımsız çekirdek)
├── C1Soft.DataAccess/       ← Katman 2: EF Core DbContext, Fluent API konfigürasyonları,
│                                        Seed Data (DbInitializer), PasswordHasher motoru ve Migration'lar
├── C1Soft.Business/         ← Katman 3: DTO modelleri, FluentValidation doğrulayıcıları,
│                                        Servis arayüzleri ve IQueryable optimize iş mantığı
└── C1Soft.WebUI/            ← Katman 4: ASP.NET Core MVC Controller'ları, Razor View'ler,
                                         Cookie Auth middleware ve wwwroot statik varlıkları
```

### Neden N-Tier Mimari Tercih Edildi?
- **Gereksiz Karmaşıklıktan Kaçınma:** Case projesinin doğasına uygun olarak; aşırı soyutlanmış, bakım maliyeti yüksek ve gereksiz kalıplar (over-engineering) yerine, saf kurumsal N-Tier tercih edilmiştir.
- **İş Mantığı İzolasyonu:** Controller sınıfları doğrudan veritabanına bağlanmaz; tüm kurallar (stok aşım kontrolü, atomik sipariş oluşturma, snapshot alma) `C1Soft.Business` servisleri arkasında yürütülür.
- **İnce Controller (Thin Controller) Prensibi:** Controller katmanı sadece HTTP isteklerini karşılar, yetki kontrolü yapar ve uygun ViewModel/JSON çıktısını döner.

---

## 4. ÖNE ÇIKAN KRİTİK B2B ÇÖZÜMLERİ

### 🔹 4.1. Veritabanından Yönetilen Dinamik B2B Grid (Madde 6.1)
B2B sistemlerinde her bayinin veya projenin ihtiyaç duyduğu kolon düzeni farklıdır. Projede kolonlar HTML içine sabit kodlanmamıştır:
- `GridColumnDefinitions` tablosunda hangi kolonun görüntüleneceği (`FieldName`), başlığı (`HeaderName`), sırası (`OrderIndex`), render tipi (`Image`, `Text`, `StockBadge`, `Price`, `QuantityInputWithCart`, `ActionModal`), genişliği, metin hizalaması ve hatta cihaza göre (Masaüstü / Tablet / Mobil) görünürlüğü veritabanından dinamik okunur.
- Yeni bir kolon eklemek veya sırasını değiştirmek için **C# kodu değiştirmeye gerek yoktur**.

### 🔹 4.2. Ürün Bazlı Dinamik Kritik Stok Eşiği (Madde 6.1)
- Sabit bir stok eşiği (örn: 10) tüm ürünler için geçerli değildir; her ürünün kendi `CriticalStockThreshold` alanı vardır.
- **Stok > Kritik Eşik** ➔ Yeşil: **"Var"**
- **0 < Stok <= Kritik Eşik** ➔ Sarı: **"Kritik"**
- **Stok <= 0** ➔ Kırmızı: **"Yok"** *(Adet kutusu ve sepet butonu otomatik kilitlenir)*

### 🔹 4.3. Fiyat & Ürün Snapshot Koruması (Madde 9)
- Amatör sistemlerde sipariş kalemleri doğrudan güncel ürün tablosuna bağlanır ve ürünün fiyatı değiştiğinde eski siparişler bozulur.
- Projemizde sipariş verildiği an ürünün o saniyedeki **adı (`ProductName`)**, **kodu (`ProductCode`)** ve **birim fiyatı (`UnitPrice`)** `OrderItems` tablosuna kalıcı olarak dondurulur (Snapshot).
- İleride ürünün fiyatı 100 TL'den 500 TL'ye çıksa veya ürün silinse dahi, geçmiş sipariş faturası **daima 100 TL olarak korunur**.

### 🔹 4.4. Çift Stok Kontrolü ve Atomik Transaction (Madde 8 & 9)
- Siparişi Onayla butonuna basıldığında yalnızca frontend JavaScript'ine güvenilmez.
- Backend'de `Database.BeginTransactionAsync()` başlatılır; sepetteki ürünlerin güncel stokları veritabanında kilitlenerek okunur.
- Eğer talep edilen adet mevcut stoktan fazlaysa işlem derhal iptal edilir (`Rollback`), sepet korunur ve kullanıcıya:  
  `"Ürün X için yeterli stok bulunmamaktadır. Mevcut stok: 5."` mesajı döndürülür.
- Stoklar yeterliyse stoklar anında düşülür, sipariş ve snapshot kalemleri kaydedilir, sepet temizlenir ve `Commit` ile atomik olarak tamamlanır.

### 🔹 4.5. SQL Seviyesinde Gelişmiş Arama Performansı (Madde 3.4 & 12)
- Tüm liste sunucu belleğine çekilerek (`context.Products.ToList().Where(...)`) RAM tüketen amatör yaklaşımdan kaçınılmıştır.
- Arama işlemi `IQueryable` zinciri üzerinden SQL Server'a `WHERE ... LIKE %query%` parametreli sorgusu olarak gönderilir; yalnızca filtreye uyan ve ekranda gösterilecek sütunlar SQL projeksiyonu (`Select`) ile çekilir.

---

## 5. KURULUM VE ÇALIŞTIRMA KILAVUZU

Proje, herhangi bir dış bağımlılık (harici SQL Server Instance, Docker, IIS vb.) gerektirmeden, Visual Studio ile yerel olarak gelen **MSSQLLocalDB** motorunu kullanarak **tek tıkla çalışır**.

### Ön Gereksinimler
- Visual Studio 2022 (v17.8 veya üzeri) veya .NET 10 SDK
- SQL Server Express LocalDB (Visual Studio "ASP.NET and web development" paketiyle birlikte otomatik gelir)

### 🚀 Adım Adım Çalıştırma

#### Seçenek A: Visual Studio ile Çalıştırma (Önerilen)
1. `C1Soft.slnx` (veya `C1Soft.sln`) dosyasını Visual Studio ile açın.
2. Başlangıç projesinin **`C1Soft.WebUI`** seçili olduğundan emin olun.
3. Klavyeden **`F5`** tuşuna basın.
4. **Veritabanı Otomatik Hazırlanır:** Uygulama ayağa kalkarken `Program.cs` içerisindeki `DbInitializer`, veritabanını (`C1SoftB2BDb`) otomatik oluşturur, migration'ları uygular ve tüm test verilerini (admin, bayi, ürünler, dinamik grid kolonları, bannerlar) tohumlar.
5. Tarayıcınız otomatik açılarak `https://localhost:7152` adresinde ana sayfa yüklenir.

#### Seçenek B: Terminal (CLI) ile Çalıştırma
```powershell
# Proje ana dizinindeyken:
cd C1Soft.WebUI
dotnet run
```
Konsolda beliren adresi (`https://localhost:7152` veya `http://localhost:5195`) tarayıcınızda açınız.

---

## 6. VERİTABANI ENTEGRASYONU VE SCRIPT BİLGİSİ

### Bağlantı Dizesi (Connection String)
Bağlantı ayarı [appsettings.json](file:///c:/Users/User/OneDrive/Masa%C3%BCst%C3%BC/C1Soft/C1Soft.WebUI/appsettings.json) dosyasında tanımlıdır:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=C1SoftB2BDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
```

### SQL Oluşturma Scripti (Madde 15.2 Teslim Şartı)
Eğer migration kullanmak yerine veritabanını doğrudan SQL Management Studio (SSMS) üzerinden elle oluşturmak isterseniz; projenin kök dizininde yer alan **`Database_Script.sql`** dosyasını SSMS üzerinde çalıştırabilirsiniz. Bu script; Primary Key'ler, Foreign Key ilişkileri, Unique kısıtları ve varsayılan değerleriyle tam şemayı üretir.

---

## 7. VARSAYILAN TEST HESAPLARI (SEED DATA)

Uygulama ilk kez ayağa kalktığında otomatik olarak aşağıdaki test hesapları oluşturulur:

| Rol | Kullanıcı Adı | Şifre | E-Posta | Panel / Erişim |
| :--- | :--- | :--- | :--- | :--- |
| **Sistem Yöneticisi (Admin)** | `admin` | `Admin123!` | `admin@c1soft.com` | `/Admin/Orders` (Sipariş Yönetimi, Ürün Yönetimi, Bayi Masası) |
| **Bayi / Müşteri (Customer)** | `bayi1` | `Bayi123!` | `bayi1@c1soft.com` | `/Home/Index` (B2B Kataloğu, Hızlı Sipariş, Sepetim, Siparişlerim) |

*(Not: Şifreler veritabanında açık metin tutulmaz; PBKDF2/SHA256 ile tuzlanmış hash olarak saklanır).*

---

## 8. CASE GEREKSİNİMLERİ KARŞILAMA MATRİSİ

| Case Maddesi | İstenen Gereksinim | Karşılanma Durumu & Teknik Çözüm |
| :---: | :--- | :--- |
| **1.1** | Ürün Yönetimi (Ekleme, Düzenleme, Zorunlu Alanlar) | ✅ `AdminController.Products`, `ProductCreate`, `ProductEdit`. Resim upload, kategori seçimi ve FluentValidation kuralları tamamlandı. |
| **2** | Kullanıcı Yönetimi & Güvenli Şifre | ✅ `AdminController.Users`, `UserEdit`. Şifreler PBKDF2 / SHA256 Salted Hash motoru ile saklanır. |
| **3** | Sipariş Yönetimi (Onay/Red, Snapshot Detay) | ✅ `AdminController.Orders`, `OrderDetail`. Sipariş durumu `Approved`/`Rejected` yapılır ve `AdminNote` bayiye iletilir. |
| **4.1** | Ana Sayfa Kampanya Slider / Banner Yönetimi | ✅ **Artı/Ekstra Puan:** `AdminController.Banners`, `BannerCreate`, `BannerEdit`, `BannerDelete`. Slider içerikleri yönetim panelinden dinamik eklenir, görsel yüklenir/silinir, sıralanır ve `Home/Index` Bootstrap Carousel ile yayınlanır. |
| **5** | Kullanıcı Kaydı, Giriş & Yetkilendirme | ✅ `AuthController` (Login/Register/Logout), Cookie Authentication ve Role-Based yetkilendirme uygulandı. |
| **6** | Ürün Arama & Detay Popup | ✅ Tüm metinsel alanlarda LIKE arama ve Bootstrap Modal Detay Popup'ı Fetch API ile yapıldı. |
| **6.1** | B2B Dinamik Grid & Kritik Stok Rozetleri | ✅ `GridColumnDefinitions` tablosuna bağlı dinamik tablo render motoru ve ürün bazlı stok rozetleri (`Var`/`Kritik`/`Yok`) tamamlandı. |
| **7** | Sepet İşlemleri (Adet, Silme, Toplam) | ✅ `CartController` AJAX uçları, dinamik Navbar sepet sayacı ve SweetAlert2 toast bildirimleri tamamlandı. |
| **8** | Çift Stok Kontrolü & Stok Aşım Uyarısı | ✅ Frontend ve backend'de `OrderService` seviyesinde çift kontrol sağlandı; yetersiz stok durumunda anlaşılır uyarı mesajı iletilir. |
| **9** | Sipariş Oluşturma, Atomik TX & Snapshot | ✅ `BeginTransactionAsync` ile sipariş anındaki ürün adı, kodu ve birim fiyatı donduruldu (Snapshot); stoklar düşürüldü. |
| **10** | Siparişlerim & Yönetici Durum Yansıması | ✅ `OrderController.MyOrders` ve `Detail`. Admin sipariş durumunu değiştirdiğinde bayinin ekranında anında güncellenir. |
| **11** | İlişkisel Veritabanı Tasarımı | ✅ PK, FK, Unique Index kısıtları ve tipleri EF Core Fluent API ile yapılandırıldı. |
| **12** | SQL Beklentisi & Performans | ✅ Bellekte filtreleme engellendi; doğrudan SQL Server'a `IQueryable` + `EF.Functions.Like` sorguları atılır. |
| **13** | Validasyon ve Hata Yönetimi | ✅ FluentValidation, model validation ve kullanıcı dostu hata ekranları hazırlandı. |
| **15** | Teslim Beklentisi | ✅ Kaynak kod, `Database_Script.sql`, detaylı `README.md` ve `docs/TEST_GUIDELINE.md` teslimata hazırlandı. |

---

> Uçtan uca test senaryoları ve adım adım tıklama rehberi için **[docs/TEST_GUIDELINE.md](docs/TEST_GUIDELINE.md)** dosyasını inceleyebilirsiniz.
