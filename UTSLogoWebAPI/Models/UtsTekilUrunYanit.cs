using Newtonsoft.Json;

namespace UTSLogoWebAPI.Models
{
    public class UtsTekilUrunYanit
    {// SNC: Sorgulanan Ürün Kayıtları Listesi (Serial Number Content)
        [JsonProperty("SNC")]
        public List<UrunDetayi> UrunListesi { get; set; }

        // MSJ: Mesaj (Hata veya bilgilendirme mesajı, null olabilir)
        [JsonProperty("MSJ")]
        public string Mesaj { get; set; }
    }
}