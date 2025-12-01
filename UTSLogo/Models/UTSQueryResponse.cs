using Newtonsoft.Json;

namespace UTSLogo.Models
{
    public class UTSQueryResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("urun")]
        public UTSUrunResponse Urun { get; set; }

        [JsonProperty("istenenAdet")]
        public int IstenenAdet { get; set; }

        [JsonProperty("stokYeterli")]
        public bool StokYeterli { get; set; }

        [JsonProperty("oncekiKontor")]
        public int OncekiKontor { get; set; }

        [JsonProperty("dusulenKontor")]
        public int DusulenKontor { get; set; }

        [JsonProperty("kalanKontor")]
        public int KalanKontor { get; set; }

        [JsonProperty("mevcutKontor")]
        public int? MevcutKontor { get; set; }

        [JsonProperty("gerekliKontor")]
        public int? GerekliKontor { get; set; }

        [JsonProperty("utsStokMiktari")]
        public int? UtsStokMiktari { get; set; }
    }

    public class UTSUrunResponse
    {
        [JsonProperty("uno")]
        public string Uno { get; set; }

        [JsonProperty("lno")]
        public string Lno { get; set; }

        [JsonProperty("utsStokMiktari")]
        public int UtsStokMiktari { get; set; }

        [JsonProperty("toplamKullanilabilirAdet")]
        public int ToplamKullanilabilirAdet { get; set; }

        [JsonProperty("kalanKullanilabilirAdet")]
        public int KalanKullanilabilirAdet { get; set; }

        [JsonProperty("uretimTarihi")]
        public string UretimTarihi { get; set; }

        [JsonProperty("sonKullanmaTarihi")]
        public string SonKullanmaTarihi { get; set; }

        [JsonProperty("urunTipi")]
        public string UrunTipi { get; set; }

        [JsonProperty("markaModel")]
        public string MarkaModel { get; set; }
    }
}