using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.Model.ViewModels
{
    public class PromoCodeEditViewModel
    {
        public int PromoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? UsageLimit { get; set; }
        public string Notes { get; set; }
    }
}
