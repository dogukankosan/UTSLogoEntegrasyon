using System.ComponentModel.DataAnnotations;

namespace UTSLogoWebAPI.Models
{
    // Yeni müşteri kaydı
    public class CustomerRegisterModel
    {
        [Required]
        [StringLength(250)]
        public string CustomerName { get; set; }

        [Required]
        public string UTSToken { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Count { get; set; }
    }

    // Kontör işlemleri (ekleme/düşme)
    public class CountOperationModel
    {
        [Required]
        public string CustomerGUID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }
    }

    // UTS Token güncelleme
    public class UTSTokenUpdateModel
    {
        [Required]
        public string CustomerGUID { get; set; }

        [Required]
        public string UTSToken { get; set; }
    }

    // Durum güncelleme
    public class StatusUpdateModel
    {
        [Required]
        public string CustomerGUID { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}