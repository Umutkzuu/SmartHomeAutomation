# 🏠 SmartHomeDB — Akıllı Ev Yönetim Sistemi

> **TBL331 Veritabanı Yönetim Sistemleri — 2025-2026 Bahar Dönemi Projesi**  
> Kocaeli Üniversitesi, Bilişim Sistemleri Mühendisliği Bölümü  
> **Grup No:** 112

---

## 👥 Grup Üyeleri

| İsim | Öğrenci No |
|------|------------|
| Umut Kuzu | 221307016 |
| Sümeyye Muratoğulları | 211307008 |

---

## 📌 Problem Tanımı

Modern yaşamda evlerdeki elektrikli cihazların sayısı hızla artmakta ve bu cihazların merkezi bir noktadan izlenmesi, yönetilmesi bir ihtiyaç haline gelmektedir. Bu proje; bir akıllı ev ortamındaki cihazların (aydınlatma, ısıtma, soğutma, güvenlik kameraları, kilit sistemleri vb.) oda bazında organize biçimde yönetilebildiği, her işlemin kayıt altına alındığı ve kullanıcı rollerine göre yetkilendirmenin sağlandığı bir **veritabanı destekli web yönetim sistemi** geliştirmektir.

Temel çözülmesi beklenen problemler şunlardır:

- Farklı odalar ve kategorilerdeki cihazların tek ekrandan açılıp kapatılabilmesi
- Oda bazında toplu işlem (tüm cihazları aç / kapat) yapılabilmesi
- Gece modu gibi senaryoların tek tıkla otomatik olarak devreye alınabilmesi
- Her cihaz değişikliğinin zaman damgasıyla birlikte loglanması
- Sistemdeki aktif cihaz durumunun anlık olarak görüntülenebilmesi

---

## 🔬 Yapılan Araştırmalar

Proje geliştirme sürecinde aşağıdaki konularda araştırmalar yapılmıştır:

- **ASP.NET Core MVC ile MySQL entegrasyonu:** `Pomelo.EntityFrameworkCore.MySql` paketi kullanılarak veritabanı bağlantısı Entity Framework Core üzerinden sağlanmıştır. DbContext yapılandırması için resmi EF Core dokümantasyonu incelenmiştir.
- **Stored Procedure ile OUTPUT parametresi:** ADO.NET üzerinden `CommandType.StoredProcedure` ve `ParameterDirection.Output` kullanımı araştırılmış; EF Core'un bu senaryoyu doğrudan desteklemediği görülerek ham bağlantı (`GetDbConnection()`) yaklaşımı tercih edilmiştir.
- **MySQL Trigger yazımı:** `AFTER UPDATE` ve `BEFORE INSERT` trigger'larının MySQL 8.x sözdizimi incelenmiş, `DELIMITER` kullanımının önemi test ortamında doğrulanmıştır.
- **Veritabanı normalizasyonu (5NF):** Tabloların 1NF'den 5NF'e kadar olan kurallara uygunluğu gözden geçirilmiş; `DEVICE_CATEGORIES` tablosu ayrıştırılarak `DEVICES` tablosundaki tekrarlayan kategori bilgilerinin kaldırılması sağlanmıştır.
- **Index optimizasyonu:** Sık sorgulanan alanlar (`DeviceName`, `Timestamp`) üzerine index oluşturmanın sorgu performansına etkisi araştırılmıştır.

---

## 🗄️ Veritabanı Şeması

### Tablolar ve İlişkiler

| Tablo | Açıklama |
|-------|----------|
| `USERS` | Sisteme erişen kullanıcılar (Admin, User, Guest rolleri) |
| `ROOMS` | Evdeki odalar ve kat bilgileri |
| `DEVICE_CATEGORIES` | Cihaz kategorileri (Aydınlatma, Isıtma vb.) ve birim sembolleri |
| `DEVICES` | Kayıtlı akıllı cihazlar; oda ve kategoriye bağlı |
| `DEVICE_LOGS` | Cihazlarda gerçekleşen her eylemin zaman damgalı kaydı |

### İlişkiler

```
ROOMS          ──< DEVICES >── DEVICE_CATEGORIES
                    │
                    └──< DEVICE_LOGS >── USERS
```

- Bir oda birden fazla cihaz barındırabilir (`1:N`)
- Bir kategori birden fazla cihazı sınıflandırabilir (`1:N`)
- Bir cihaz birden fazla log kaydı üretebilir (`1:N`)
- Bir kullanıcı birden fazla log kaydını tetikleyebilir (`1:N`)

---

## 🗺️ ER Diyagramı

```
USERS               DEVICE_LOGS             DEVICES
─────────           ───────────             ───────
UserID (PK)    ─┐   LogID (PK)        ┌─── DeviceID (PK)
FirstName       └── UserID (FK)       │    DeviceName
LastName            DeviceID (FK) ────┘    RoomID (FK) ────── ROOMS
Email               ActionType             CategoryID (FK) ─── DEVICE_CATEGORIES
PasswordHash        OldValue               IsActive
Role                NewValue               CurrentValue
CreatedAt           Timestamp              IsOnline
```

> ER diyagramı detaylı görseli için proje klasöründeki `ER_Diyagrami.png` dosyasına bakınız.

---

## ⚙️ Gelişmiş Veritabanı Nesneleri

### 📋 View'lar (Görünümler)

| View Adı | Amaç |
|----------|------|
| `vw_ActiveDevicesDashboard` | Aktif durumdaki tüm cihazları oda, kategori ve birim bilgileriyle listeler |
| `vw_RoomDeviceSummary` | Her oda için toplam cihaz sayısı ve aktif cihaz sayısını özetler |

### ⚡ Trigger'lar (Tetikleyiciler)

| Trigger Adı | Olay | Amaç |
|------------|------|------|
| `trg_AfterDeviceUpdate` | `AFTER UPDATE` (DEVICES) | Cihaz durumu veya değeri değiştiğinde otomatik log oluşturur |
| `trg_BeforeUserInsert` | `BEFORE INSERT` (USERS) | Kayıt öncesinde e-posta adresini otomatik olarak küçük harfe dönüştürür; veri bütünlüğünü (Data Integrity) arayüzden bağımsız korumak ve gelecekte eklenecek Authentication modülüne veritabanı seviyesinde altyapı hazırlamak amacıyla kurgulanmıştır. |

### 🔧 Stored Procedure'lar (Saklı Yordamlar)

| Procedure Adı | Parametreler | Amaç |
|--------------|-------------|------|
| `sp_TurnOffRoomDevices` | `IN p_RoomID INT` | Belirtilen odadaki tüm aktif cihazları kapatır |
| `sp_TurnOnRoomDevices` | `IN p_RoomID INT` | Belirtilen odadaki tüm çevrimiçi cihazları açar |
| `sp_ActivateNightMode` | `OUT p_StatusMessage VARCHAR` | Transaction ile gece modunu aktif eder; ışıkları kapatır, güvenlik cihazlarını devreye alır, ısıtmayı 18°C'ye ayarlar |

### 🔍 Index'ler

| Index Adı | Tablo | Sütun | Amaç |
|----------|-------|-------|------|
| `idx_DeviceName` | DEVICES | DeviceName | Cihaz adı bazlı aramalarda performans iyileştirmesi |
| `idx_LogTimestamp` | DEVICE_LOGS | Timestamp | Zamana göre log sorgularında hız artışı |

---

## 🏗️ Yazılım Mimarisi

Proje **N-Katmanlı (N-Tier) Mimari** prensibiyle geliştirilmiştir:

```
┌─────────────────────────────────────────────────────┐
│                  SUNUM KATMANI                      │
│         ASP.NET Core MVC — Razor Views              │
│   Index.cshtml  │  Logs/Index.cshtml               │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│                 KONTROL KATMANI                     │
│              ASP.NET Core Controllers               │
│   HomeController.cs  │  LogsController.cs          │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│               VERİ ERİŞİM KATMANI                  │
│        Entity Framework Core (Code-First)           │
│   SmartHomeDbContext.cs  │  Entity Classes          │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│                 VERİTABANI KATMANI                  │
│              MySQL 8.x — SmartHomeDB                │
│   Tables │ Views │ Triggers │ Procedures │ Indexes  │
└─────────────────────────────────────────────────────┘
```

**Kullanılan Teknolojiler:**

- **Backend:** ASP.NET Core MVC (.NET 8)
- **ORM:** Entity Framework Core + Pomelo.EntityFrameworkCore.MySql
- **Veritabanı:** MySQL 8.x
- **Frontend:** Razor Pages, Bootstrap 5, Bootstrap Icons
- **IDE:** Visual Studio / Visual Studio Code

---

## 🔄 Akış Şeması

### Ana Kontrol Paneli Akışı

```
Kullanıcı → Tarayıcı
     │
     ▼
HomeController.Index()
     │
     ├── EF Core → DEVICES tablosu sorgusu
     │       │
     │       └── ViewBag'e Toplam / Aktif / Çevrimdışı sayıları aktarılır
     │
     ▼
Index.cshtml render edilir
     │
     ├── [Cihaz Aç/Kapat]  → POST ToggleDevice(id)
     │        └── Device.IsActive ters çevrilir
     │             └── trg_AfterDeviceUpdate otomatik log yazar
     │
     ├── [Oda Tümünü Aç]   → POST BulkRoomAction(roomId, status=true)
     │        └── CALL sp_TurnOnRoomDevices(roomId)
     │
     ├── [Oda Tümünü Kapat] → POST BulkRoomAction(roomId, status=false)
     │        └── CALL sp_TurnOffRoomDevices(roomId)
     │
     └── [Gece Modunu Başlat] → POST ActivateNightMode()
              └── CALL sp_ActivateNightMode(OUT mesaj)
                   ├── START TRANSACTION
                   ├── Aydınlatma & Eğlence → KAPAT
                   ├── Güvenlik & Kilit → AÇ
                   ├── Isıtma → 18°C
                   └── COMMIT / ROLLBACK
```

---

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [MySQL Server 8.x](https://dev.mysql.com/downloads/mysql/)
- Git

### Adım 1 — Depoyu Klonlayın

```bash
git clone https://github.com/[kullanici-adi]/SmartHomeDB.git
cd SmartHomeDB
```

### Adım 2 — Veritabanını Kurun

MySQL istemcinizde sırasıyla aşağıdaki betikleri çalıştırın:

```bash
# Önce temel yapıyı ve test verilerini oluşturun
mysql -u root -p < init_db.sql

# Ardından gelişmiş nesneleri (View, Trigger, Procedure, Index) ekleyin
mysql -u root -p < advanced_objects.sql
```

### Adım 3 — Bağlantı Dizesini Güncelleyin

`SmartHomeDbContext.cs` dosyasındaki bağlantı dizesini kendi ortamınıza göre düzenleyin:

```csharp
optionsBuilder.UseMySql(
    "server=127.0.0.1;port=3306;database=SmartHomeDB;user=root;password=SIFRENIZ",
    ...
);
```

> ⚠️ Bağlantı dizesini üretim ortamında `appsettings.json` veya ortam değişkenlerine taşıyınız.

### Adım 4 — Uygulamayı Başlatın

```bash
dotnet run --project SmartHome.Web
```

Tarayıcınızda `https://localhost:5001` adresini açın.

---

## 🖥️ Arayüz Görselleri

> *(GitHub reposuna `screenshots/` klasörü altına eklenecektir.)*

| Ekran | Açıklama |
|-------|----------|
| Ana Kontrol Paneli | Toplam / Aktif / Çevrimdışı cihaz özetleri ve oda bazlı cihaz listesi |
| Log Ekranı | Tüm cihaz eylemlerinin zaman damgalı kayıtları |
| Gece Modu | `sp_ActivateNightMode` çıktı mesajı ve otomatik durum değişiklikleri |

---

## 📐 Normalizasyon (5NF Uyumu)

Veritabanı tasarımı, her normal formun kuralları tek tek kontrol edilerek 5NF seviyesine taşınmıştır. En kritik adım, kategori bilgilerinin `DEVICES` tablosundan ayrıştırılarak `DEVICE_CATEGORIES` adlı bağımsız bir tabloya alınmasıdır. Bu sayede aynı kategorinin (`Aydınlatma`, `Isıtma` vb.) her cihaz kaydında tekrar etmesi engellenmiş; `CategoryName` ve `UnitSymbol` gibi alanların tek bir kaynaktan yönetilmesi sağlanmıştır. Bu ayrışma, `DEVICES → DEVICE_CATEGORIES` arasındaki Join Dependency'yi birincil anahtarlar üzerinden tanımlı hale getirerek tasarımı BCNF ve ardından 4NF/5NF gereksinimlerini karşılar konuma getirmiştir.

| Normal Form | Durum | Açıklama |
|-------------|:-----:|----------|
| **1NF** | ✅ | Tüm sütunlar atomik değerler taşımaktadır; tekrar eden sütun grubu ya da dizi veri tipi bulunmamaktadır. |
| **2NF** | ✅ | Hiçbir tabloda bileşik birincil anahtar kullanılmamıştır; dolayısıyla kısmi fonksiyonel bağımlılık (Partial Dependency) söz konusu değildir. |
| **3NF** | ✅ | Geçişli bağımlılık (Transitive Dependency) giderilmiştir. Kategori adı ve birim sembolü `DEVICES` tablosundan çekilerek `DEVICE_CATEGORIES` tablosuna taşınmıştır. |
| **BCNF** | ✅ | Her tablodaki tüm determinantlar aday anahtar (Candidate Key) niteliğindedir; bu kuralı ihlal eden herhangi bir fonksiyonel bağımlılık tespit edilmemiştir. |
| **4NF** | ✅ | Tablolarda bağımsız çok değerli bağımlılık (Multi-Valued Dependency) bulunmamaktadır. Her tablo tek bir olguyu (entity/fact) temsil etmektedir. |
| **5NF** | ✅ | Tüm Join Dependency ilişkileri, ilgili tabloların birincil anahtarlarından türetilmektedir. `DEVICE_CATEGORIES` tablosunun ayrıştırılması bu uyumu doğrudan desteklemiştir. |

---

## 📚 Referanslar

1. Microsoft. *ASP.NET Core MVC Documentation.* https://learn.microsoft.com/en-us/aspnet/core/mvc/
2. Pomelo Foundation. *Pomelo.EntityFrameworkCore.MySql GitHub Repository.* https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
3. Oracle. *MySQL 8.0 Reference Manual — Stored Procedures and Triggers.* https://dev.mysql.com/doc/refman/8.0/en/
4. Microsoft. *Entity Framework Core — Raw SQL Queries.* https://learn.microsoft.com/en-us/ef/core/querying/raw-sql
5. Ramez Elmasri, Shamkant Navathe. *Fundamentals of Database Systems, 7th Edition.* Pearson, 2015.
6. Bootstrap. *Bootstrap 5 Documentation.* https://getbootstrap.com/docs/5.0/

---

> **Not:** Bu proje TBL331 Veritabanı Yönetim Sistemleri dersi kapsamında akademik amaçla geliştirilmiştir.