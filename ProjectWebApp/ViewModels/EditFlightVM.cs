using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.ViewModels
{
    public class EditFlightVM : IValidatableObject
    {
        public int FlightId { get; set; }

        [Required(ErrorMessage = "Flight number is required")]
        public string FlightNumber { get; set; }

        [Required(ErrorMessage = "Aircraft is required")]
        public string Aircraft { get; set; }

        [Required(ErrorMessage = "Base price is required")]
        [Range(0.01, 100000, ErrorMessage = "Base price must be greater than 0")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int StatusId { get; set; }

        public List<SelectListItem> Statuses { get; set; }

        // Read-only display
        public string AirlineName { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }

        // ✅ CUSTOM VALIDATION
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ArrivalTime <= DepartureTime)
            {
                yield return new ValidationResult(
                    "Arrival time must be after departure time",
                    new[] { nameof(ArrivalTime) }
                );
            }
        }
    }
}
