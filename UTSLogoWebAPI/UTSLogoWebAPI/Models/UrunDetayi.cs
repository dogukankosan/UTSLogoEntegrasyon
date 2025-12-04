using Newtonsoft.Json;

namespace UTSLogoWebAPI.Models
{
    public class UrunDetayi
    {
        [JsonProperty("UNO")]
        public string Uno { get; set; } // Ürün Numarası
        [JsonProperty("LNO")]
        public string Lno { get; set; } // Lot Numarası
        [JsonProperty("ADT")]
        public int Adet { get; set; } // ADT: Adet (Stok Miktarı)

        [JsonProperty("URT")]
        public string UretimTarihiString { get; set; } // URT: Üretim Tarihi (string)
        [JsonIgnore]
        public DateTime UretimTarihi => DateTime.ParseExact(UretimTarihiString, "yyyy-MM-dd", null);

        [JsonProperty("SKT")]
        public string SonKullanmaTarihiString { get; set; } // SKT: Son Kullanma Tarihi (string)
        [JsonIgnore]
        public DateTime SonKullanmaTarihi => DateTime.ParseExact(SonKullanmaTarihiString, "yyyy-MM-dd", null);

        [JsonProperty("MME")]
        public string MarkaModelEtiket { get; set; } // MME: Marka Model Etiketi

        [JsonProperty("UTP")]
        public string UrunTipi { get; set; } // UTP: Ürün Tipi ("TIBBI_CIHAZ" vb.)

        [JsonProperty("UIK")]
        public long UrunIciKod { get; set; } // UIK: Ürün İçi Kod (UTS'nin iç kodu)

        [JsonProperty("UAK")]
        public string UrunAyrimKodu { get; set; } // UAK: Ürün Ayrım Kodu ("LOT" veya "SERI" vb.)

        [JsonProperty("KKG")]
        public bool KayitliKullanimaGonderilebilir { get; set; } // KKG: Kayıtlı Kullanıma Gönderilebilir (true/false)

        [JsonProperty("TKA")]
        public int ToplamKullanilabilirAdet { get; set; } // TKA: Toplam Kullanılabilir Adet

        [JsonProperty("KKA")]
        public int KullanilabilirKalanAdet { get; set; } // KKA: Kullanılabilir Kalan Adet (Genellikle TKA ile aynıdır)

        [JsonProperty("IUS")]
        public int IptalUretimSebebi { get; set; } // IUS: İptal Üretim Sebebi (0 veya 1: Kayıtlı/Kayıtsız)
    }
}
