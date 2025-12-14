using Newtonsoft.Json;

namespace UTSLogoWebAPI.Models
{
    public class UtsTekilUrunYanit
    {
        [JsonProperty("SNC")]
        public List<UrunDetayi> UrunListesi { get; set; }
        [JsonProperty("MSJ")]
        public string Mesaj { get; set; }
    }
}