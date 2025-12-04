# 📦 UTSLogoEntegrasyon

![License](https://img.shields.io/github/license/dogukankosan/UTSLogoEntegrasyon)
![Stars](https://img.shields.io/github/stars/dogukankosan/UTSLogoEntegrasyon)
![Issues](https://img.shields.io/github/issues/dogukankosan/UTSLogoEntegrasyon)
![Last Commit](https://img.shields.io/github/last-commit/dogukankosan/UTSLogoEntegrasyon)

> **UTSLogoEntegrasyon**, Logo ERP sistemi ile T.C. Sağlık Bakanlığı Ürün Takip Sistemi (UTS) arasında entegrasyon sağlayan, satış faturalarından otomatik ürün bilgisi çeken ve PTSNOTICE kayıtlarını oluşturan bir masaüstü C#/.NET uygulaması ve Web API servisidir.

---

## 🚀 Özellikler

- 🔗 Logo ERP ile tam entegrasyon (Satış faturaları, stok kartları)
- 📡 UTS (Ürün Takip Sistemi) API entegrasyonu
- 🏢 Çoklu müşteri ve firma desteği
- 💳 Kontör bazlı kullanım sistemi (sorgu başına kontör düşme)
- 📋 Satış faturalarından otomatik UNO/LOT bilgisi çekme
- 📅 Üretim tarihi ve son kullanma tarihi otomatik güncelleme
- 📊 DevExpress grid ile modern fatura listeleme
- ✅ PTSNOTICE tablolarına otomatik kayıt atma
- 🗄 SQLite ile yerel kayıt takibi (çekilmiş faturalar)
- 🌐 RESTful Web API servisi (müşteri yönetimi, UTS sorgulaması)
- 🔐 GUID bazlı müşteri kimlik doğrulama
- 📝 Detaylı hata loglama sistemi

---

## 🗂 Proje Yapısı

```
UTSLogoEntegrasyon/
├── Controllers/
│   ├── CustomersController.cs    # Müşteri CRUD işlemleri API
│   └── UTSController.cs          # UTS sorgu API endpoint'i
├── Forms/
│   └── SalesInvoicesForm.cs      # Satış faturaları ve UTS çekim ekranı
├── Classes/
│   ├── SQLCrud.cs                # SQL Server veritabanı işlemleri
│   ├── SQLiteCrud.cs             # SQLite yerel veritabanı işlemleri
│   ├── UTSApiClient.cs           # UTS API istemcisi
│   └── TextLog.cs                # Loglama sınıfı
├── Models/
│   ├── CustomerRegisterModel.cs  # Müşteri kayıt modeli
│   ├── CountOperationModel.cs    # Kontör işlem modeli
│   ├── UTSQueryRequestModel.cs   # UTS sorgu istek modeli
│   └── ...                       # Diğer modeller
└── ...
```

---

## 🛠️ Kurulum & Çalıştırma

### Web API Servisi

1. **Projeyi Klonla:**
   ```bash
   git clone https://github.com/dogukankosan/UTSLogoEntegrasyon.git
   cd UTSLogoEntegrasyon
   ```

2. **Veritabanı Bağlantısını Ayarla:**
   - `appsettings.json` dosyasında SQL Server bağlantı cümlesini düzenle.

3. **Projeyi Çalıştır:**
   ```bash
   dotnet run
   ```

### Masaüstü Uygulaması

1. **Visual Studio ile projeyi aç ve çalıştır (`F5`).**
2. **Firma ve dönem seçimi yap.**
3. **Satış faturaları listesinden UTS çekimi yapılacak faturayı seç.**
4. **"UTS Çek" butonuna tıkla ve işlemlerin tamamlanmasını bekle.**

---

## 📡 API Endpoint'leri

### Müşteri İşlemleri (`/api/Customers`)

| Metod | Endpoint | Açıklama |
|-------|----------|----------|
| POST | `/Register` | Yeni müşteri kaydı |
| GET | `/Info?guid={guid}` | Müşteri bilgisi sorgulama |
| PUT | `/AddCount` | Kontör ekleme |
| PUT | `/DeductCount` | Kontör düşme |
| PUT | `/UpdateUTSToken` | UTS token güncelleme |
| PUT | `/SetStatus` | Müşteri durumu değiştirme |

### UTS Sorgulaması (`/api/UTS`)

| Metod | Endpoint | Açıklama |
|-------|----------|----------|
| POST | `/Query` | UTS ürün sorgulaması (UNO, LNO, SN ile) |

---

## ⚡ Kullanım Senaryosu

1. **Müşteri Kaydı:** API üzerinden müşteri oluştur ve GUID al.
2. **Kontör Yükle:** Müşteriye kontör tanımla.
3. **UTS Token Ekle:** Sağlık Bakanlığı'ndan alınan UTS token'ı sisteme kaydet.
4. **Fatura Seç:** Masaüstü uygulamasında satış faturasını seç.
5. **UTS Çek:** Faturadaki ürünlerin UTS bilgilerini otomatik çek.
6. **Sonuç:** Üretim/SKT tarihleri güncellenir, PTSNOTICE kayıtları oluşturulur.

---

## 📊 Veritabanı Tabloları

| Tablo | Açıklama |
|-------|----------|
| `Customers` | Müşteri bilgileri ve kontör durumu |
| `UTSCekimKayitlari` | UTS'den çekilen kayıtların logu (SQLite) |
| `LG_XXX_PTSNOTICE` | Logo PTS bildirim tablosu |
| `LG_XXX_YY_STLINE` | Logo stok satırları |
| `LG_XXX_YY_INVOICE` | Logo fatura tablosu |

---

## 🔧 Gereksinimler

- .NET 6.0 veya üzeri
- SQL Server 2016+
- Logo Tiger/Go ERP
- DevExpress WinForms (masaüstü uygulama için)
- UTS API erişim yetkisi ve token

---

## 🤝 Katkı

Katkı sağlamak için projeyi forklayabilir ve pull request gönderebilirsiniz.

---

## 📄 Lisans

MIT License

---

## 📬 İletişim

- 👨‍💻 Geliştirici: [@dogukankosan](https://github.com/dogukankosan)  
- 🐞 Öneri veya sorunlar: [Issues sekmesi](https://github.com/dogukankosan/UTSLogoEntegrasyon/issues)

---

<p align="center">
  <img src="https://img.shields.io/badge/.NET-6.0+-purple?logo=dotnet" alt="dotnet" />
  <img src="https://img.shields.io/badge/ASP.NET-Core%20API-blue?logo=dotnet" alt="aspnet" />
  <img src="https://img.shields.io/badge/Windows%20Forms-DevExpress-orange" alt="winforms" />
  <img src="https://img.shields.io/badge/Logo-ERP-green" alt="logo" />
  <img src="https://img.shields.io/badge/UTS-Entegrasyon-red" alt="uts" />
</p>
