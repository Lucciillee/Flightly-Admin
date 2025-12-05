using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectWebApp.ViewModels
{
    public class EditFlightVM
    {
        public int FlightId { get; set; }

        public string FlightNumber { get; set; }
        public string Aircraft { get; set; }
        public decimal BasePrice { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int StatusId { get; set; }
        public List<SelectListItem> Statuses { get; set; }

        // Read-only display
        public string AirlineName { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
    }
}
