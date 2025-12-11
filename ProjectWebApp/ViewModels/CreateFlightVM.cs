using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.ViewModels
{
    public class CreateFlightVM : IValidatableObject
    {
        [Required(ErrorMessage = "Please select an airline")]
        public int AirlineId { get; set; }

        [Required(ErrorMessage = "Flight number is required")]
        public string FlightNumber { get; set; }

        [Required(ErrorMessage = "Aircraft type is required")]
        public string Aircraft { get; set; }

        [Required(ErrorMessage = "Please select an origin airport")]
        public int OriginAirportId { get; set; }

        [Required(ErrorMessage = "Please select a destination airport")]
        public int DestinationAirportId { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Base price is required")]
        [Range(0.01, 100000, ErrorMessage = "Base price must be greater than 0")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Please select a status")]
        public int StatusId { get; set; }

        public List<SelectListItem> Airlines { get; set; }
        public List<SelectListItem> Airports { get; set; }
        public List<SelectListItem> Statuses { get; set; }

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
