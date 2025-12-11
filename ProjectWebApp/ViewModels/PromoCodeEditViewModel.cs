using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.Model.ViewModels
{
    public class PromoCodeEditViewModel : IValidatableObject
    {
        public int PromoId { get; set; }

        [Required(ErrorMessage = "Promo code is required")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Value is required")]
        [Range(0.01, 100000, ErrorMessage = "Value must be greater than 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be at least 1")]
        public int? UsageLimit { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        // ✅ SAME CUSTOM VALIDATION AS CREATE
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be later than start date",
                    new[] { nameof(EndDate) }
                );
            }
        }
    }
}
