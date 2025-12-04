using System.ComponentModel.DataAnnotations;

namespace UTSLogoWebAPI.Models
{
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
    public class CountOperationModel
    {
        [Required]
        public string CustomerGUID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }
    }
    public class UTSTokenUpdateModel
    {
        [Required]
        public string CustomerGUID { get; set; }

        [Required]
        public string UTSToken { get; set; }
    }
    public class StatusUpdateModel
    {
        [Required]
        public string CustomerGUID { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}