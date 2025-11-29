using Newtonsoft.Json;

namespace UTSLogo.Models 
{
    public class TekilUrunSorguRequest
    {
        [JsonProperty("UNO")]
        public string Uno { get; set; } // Ürün Numarası

        [JsonProperty("LNO")]
        public string Lno { get; set; } // Lot Numarası

        [JsonProperty("SAN")]
        public string San { get; set; } // Seri/Barkod Numarası
    }
}