using Newtonsoft.Json;

namespace UTSLogoWebAPI.Models
{
    public class TekilUrunSorguRequest
    {
        [JsonProperty("UNO")]
        public string Uno { get; set; }
        [JsonProperty("LNO")]
        public string Lno { get; set; }
        [JsonProperty("SAN")]
        public string San { get; set; }
    }
}
