using Newtonsoft.Json;

namespace UTSLogoWebAPI.Models
{
    public class TekilUrunSorguRequest
    {
        [JsonProperty("UNO")]
        public string Uno { get; set; }
        [JsonProperty("LNO")]
        public string Lno { get; set; }
        [JsonProperty("SAN")] // Sorgu metodunuzdaki parametre adı (seri numara)
        public string San { get; set; }
    }
}
