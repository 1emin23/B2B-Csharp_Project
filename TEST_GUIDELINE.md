# C1Soft Mini B2B E-Ticaret — Test & Değerlendirme Kılavuzu (Test Guideline)

> **Bu kılavuzun amacı:** Projeyi inceleyen değerlendiricinin veya teknik ekibin, uygulamayı sıfır konfigürasyonla çalıştırıp firma case'inde istenen tüm B2B gereksinimlerini uçtan uca, kolay ve hızlı bir şekilde test edebilmesini sağlamaktır.

---

## 1. PROJEYİ ÇALIŞTIRMA (HIZLI BAŞLANGIÇ)

Uygulama, dışarıdan herhangi bir Docker konteyneri veya harici SQL Server kurulumu gerektirmeden, Visual Studio ile birlikte gelen **Microsoft SQL Server LocalDB** üzerinde tek tıkla çalışacak şekilde yapılandırılmıştır.

### Yöntem A: Visual Studio 2022+ ile Çalıştırma (Önerilen)
1. Kök dizindeki `C1Soft.slnx` (veya `C1Soft.sln`) dosyasını Visual Studio'da açın.
2. Üst araç çubuğunda başlangıç projesinin **`C1Soft.WebUI`** olduğundan emin olun.
3. Klavyeden **`F5`** tuşuna basın (veya yeşil *Oynat* butonuna tıklayın).
4. **Veritabanı otomatik oluşturulur** (`DbInitializer` ilk açılışta tüm tabloları, test kullanıcılarını, kategorileri, 8 adet B2B ürününü, dinamik kolon konfigürasyonlarını ve banner slider'ları otomatik tohumlar).
5. Tarayıcınız kendiliğinden açılarak `https://localhost:7152` adresine gidecektir.

### Yöntem B: Terminal / Komut Satırı ile Çalıştırma
```powershell
cd C1Soft.WebUI
dotnet run
```
Konsol çıktısında belirtilen adresi (`https://localhost:7152` veya `http://localhost:5195`) tarayıcınızda açınız.

---

## 2. HAZIR TEST HESAPLARI (SEED DATA)

Uygulama ilk kez ayağa kalktığında aşağıdaki hesaplar otomatik olarak PBKDF2/SHA256 ile tuzlanmış hash olarak veritabanına işlenir:

| Rol | Kullanıcı Adı | Şifre | E-Posta | Erişim Kapsamı |
| :--- | :--- | :--- | :--- | :--- |
| **Yönetici (Admin)** | `admin` | `Admin123!` | `admin@c1soft.com` | Sipariş Onay/Red, Ürün Ekle/Düzenle, Bayi & Kullanıcı Yönetimi |
| **Bayi (Customer)** | `bayi1` | `Bayi123!` | `bayi1@c1soft.com` | B2B Ürün Kataloğu, Hızlı Sipariş Girişi, Sepet, Siparişlerim & Snapshot Fatura |

*(Not: Giriş ekranında bu hesap bilgileri tek tıkla incelenebilmesi için hazır bilgi kartı olarak da yer almaktadır).*

---

## 3. ADIM ADIM UÇTAN UCA TEST SENARYOLARI

### 🟢 SENARYO 1: Bayi Girişi & Ana Sayfa Banner Slider (Madde 4.1 & 5)
1. Sağ üstteki **"Giriş Yap"** butonuna tıklayın.
2. `bayi1` / `Bayi123!` bilgileriyle giriş yapın.
3. **Beklenen Sonuç:**
   - Şartname Madde 4.1 ve 5 gereği kullanıcı başarıyla **Ana Sayfa**'ya yönlendirilir.
   - Ana sayfada veritabanından okunan kampanyalı ürünler için dinamik **Slider / Banner Carousel** alanı gösterilir.
   - Sağ üstte bayi adı (`bayi1 (Bayi)`) ve dinamik sepet rozeti (`Sepetim (0)`) belirir.

---

### 🟢 SENARYO 2: B2B Dinamik Tablo (Grid) & Dinamik Stok Rozetleri (Madde 6.1)
1. Üst menüden **"Ürün Kataloğu"** bağlantısına tıklayın.
2. **Dinamik Grid Testi:** Tablo kolonlarının sırası, genişliği, başlığı ve hizalaması veritabanındaki `GridColumnDefinitions` tablosundan dinamik render edilir (sabit HTML kodlaması değildir).
3. **Ürün Bazlı Dinamik Stok Rozeti Testi:**
   - `HRD-1001` (Bosch Matkap) ➔ Stok: 45, Eşik: 10 ➔ **Yeşil Rozet: "Var (45)"**
   - `HRD-1002` (Makita Taşlama) ➔ Stok: 4, Eşik: 10 ➔ **Sarı Rozet: "Kritik (4)"** *(Ürünün stoğu kritik eşiğin altına düştüğü için)*
   - `ELK-2002` (Philips LED Armatür) ➔ Stok: 0, Eşik: 5 ➔ **Kırmızı Rozet: "Yok (0)"**
4. **Tükenmiş Ürün Davranışı:** Stoğu 0 olan ürün satırında adet giriş kutusu ve sipariş butonu yerine otomatik olarak devre dışı **"Tükendi"** rozeti görüntülenir.

---

### 🟢 SENARYO 3: SQL Düzeyinde Optimize Arama (Madde 3.4 & 12)
1. Ürün Kataloğu sayfasındaki arama kutusuna şu ifadeleri yazıp **"Ara"** butonuna basın:
   - Marka ile ara: `Bosch` ➔ Yalnızca Bosch markalı ürünler listelenir.
   - Üretici kodu ile ara: `06019H5100` ➔ İlgili ürün bulunur.
   - Metinsel açıklama veya özel kod ile ara: `darbe` veya `OZEL-HRD` ➔ İlgili ürünler listelenir.
2. **Teknik Doğrulama:** Arama sorgusu belleğe tüm listeyi çekip C# içinde (`ToList().Where()`) **yapılmaz**; doğrudan SQL Server'a `WHERE ... LIKE %query%` parametresiyle `IQueryable` olarak iletilir.

---

### 🟢 SENARYO 4: Ürün Detay Popup / Modal (Madde 6)
1. Listeden herhangi bir ürünün en sağındaki **"Detay"** butonuna tıklayın.
2. **Beklenen Sonuç:** Sayfa yenilenmeden Bootstrap Modal açılır; ürünün görseli, kodu, üretici kodu, özel kod 1 & 2, stok durumu ve detaylı açıklaması asenkron Fetch API ile yüklenir.

---

### 🟢 SENARYO 5: Doğrudan Tablodan Hızlı Sipariş & AJAX Sepet (Madde 3.2 & 7)
1. Tabloda bol stoklu bir ürünün (örn: `HRD-1001`) yanındaki adet kutusuna `2` yazıp **"Ekle"** butonuna basın.
2. Sayfa yenilenmeden SweetAlert2 bildiriminin geldiğini ve sağ üstteki **Sepetim (2)** sayacının anında güncellendiğini gözlemleyin.
3. Sağ üstten **"Sepetim"** butonuna tıklayın:
   - Adet artırma (`+`) veya azaltma (`-`) butonlarına basarak tutarın dinamik güncellendiğini test edin.
   - **Stok Aşımı Kontrolü:** Adet kutusuna mevcut stoktan büyük bir değer (örn: 999) girdiğinizde sistemin hata fırlattığını ve siparişi engellediğini görün.

---

### 🟢 SENARYO 6: Atomik Sipariş Oluşturma & Çift Stok Kontrolü (Madde 8 & 9)
1. Sepet sayfasında sağdaki **"Siparişi Onayla"** butonuna tıklayın.
2. **Beklenen Sonuç:**
   - Backend `Database.BeginTransactionAsync()` ile veritabanı transaction'ı başlatır.
   - Sepetteki ürünlerin güncel stokları tekrar kilitlenip okunur (Race condition çift kontrolü).
   - Sipariş anındaki ürün adı, kodu ve birim fiyatı kalıcı olarak dondurulur (Snapshot).
   - İlgili ürünün veritabanındaki stok miktarı sipariş adedi kadar anında düşürülür.
   - Bayinin sepeti temizlenir.
   - Ekranda sipariş onay mesajı ve benzersiz sipariş takip numarası (örn: `ORD-20260922-XXXXXX`) görüntülenir.

---

### 🟢 SENARYO 7: Siparişlerim & Snapshot Fiyat Koruma (Madde 9 & 10)
1. Üst menüden **"Siparişlerim"** bağlantısına tıklayın.
2. Az önce oluşturulan sipariş listede **"Beklemede"** durumuyla listelenir.
3. **"Detay"** butonuna tıklayarak sipariş faturasını açın.
4. **Snapshot Testi:** Burada görünen birim fiyat ve ürün adı, ileride admin tarafından ana ürün tablosundaki fiyat değiştirilse dahi **asla değişmez**; sipariş anındaki orijinal fiyat korunur.

---

### 🟢 SENARYO 8: Yönetim Paneli — Sipariş Onay/Red & Yönetici Notu (Madde 1 & 3)
1. Sağ üstteki profil dropdown'ından **"Çıkış Yap"** deyin.
2. **Admin** hesabı ile giriş yapın: `admin` / `Admin123!`
3. Sistem Admin'i doğrudan **`/Admin/Orders` (Sipariş Masası)** ekranına yönlendirir.
4. Üst menüde sarı renkli **"Yönetim Paneli"** açılır.
5. Bayinin az önce verdiği siparişin yanındaki **"Yönet"** butonuna tıklayın:
   - Sipariş Durumunu: **"Onaylandı"** seçin.
   - Yönetici Notu alanına: *"Siparişiniz onaylanmıştır, ürünler depodan sevk ediliyor."* yazın.
   - **"Durumu ve Notu Kaydet"** butonuna basın.
6. Admin hesabından çıkış yapıp tekrar `bayi1` ile giriş yapıp **"Siparişlerim"** sayfasına bakın:
   - Durumun anında yeşil **"Onaylandı"** rozetine dönüştüğünü ve adminin yazdığı açıklama notunun faturada görüntülendiğini doğrulayın.

---

### 🟢 SENARYO 9: Yönetim Paneli — Ürün Yönetimi (Madde 1.1)
1. Admin menüsünden **"Ürün Yönetimi"** (`/Admin/Products`) sayfasına gidin.
2. **"Yeni Ürün Ekle"** butonuna basın:
   - Ürün Kodu, Ürün Adı, Marka, Kategori, Fiyat, Stok ve Kritik Stok Eşiği alanlarını doldurun.
   - Bilgisayarınızdan bir ürün görseli seçin (`IFormFile` upload).
   - Kaydettiğinizde yeni ürünün başarıyla listeye eklendiğini ve bayi kataloğuna yansıdığını görün.
3. Mevcut bir ürünü **"Düzenle"** ile güncelleyin veya **"Sil"** butonu ile veri bütünlüğünü bozmadan güvenli şekilde pasife alın (Soft-delete).

---

### 🟢 SENARYO 10: Yönetim Paneli — Bayi & Kullanıcı Yönetimi (Madde 2)
1. Admin menüsünden **"Bayi & Kullanıcılar"** (`/Admin/Users`) sayfasına gidin.
2. Kayıtlı tüm kullanıcıları listeleyin ve arama yapın.
3. **"Düzenle"** butonuna basarak bir kullanıcının adını, e-postasını veya rolünü (`Customer` ↔ `Admin`) güncelleyin.
4. İsteğe bağlı olarak yeni bir şifre girip güvenli PBKDF2 tuzlanmış hash olarak veritabanında güncellendiğini test edin.

---

## 4. TEKNİK MİMARİ VE VERİTABANI ÖZETİ

- **Mimari:** 4 Katmanlı N-Tier (`C1Soft.Domain`, `C1Soft.DataAccess`, `C1Soft.Business`, `C1Soft.WebUI`)
- **Veritabanı Scripti:** Kök dizinde yer alan `Database_Script.sql` dosyası üzerinden tüm tablolar, foreign key'ler, index kısıtları ve veri tipleri doğrudan incelenebilir.
- **Güvenlik:** Şifreler düz metin tutulmaz; PBKDF2 / SHA256 Salted Hash algoritması ile korunmaktadır.
