using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.ViewModels
{
    public class CreateFlightVM
    {
        [Required]
        public int AirlineId { get; set; }

        [Required]
        public string FlightNumber { get; set; }

        public string Aircraft { get; set; }

        [Required]
        public int OriginAirportId { get; set; }

        [Required]
        public int DestinationAirportId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public decimal BasePrice { get; set; }

        [Required]
        public int StatusId { get; set; }

        public List<SelectListItem> Airlines { get; set; }
        public List<SelectListItem> Airports { get; set; }
        public List<SelectListItem> Statuses { get; set; }
    }
}
