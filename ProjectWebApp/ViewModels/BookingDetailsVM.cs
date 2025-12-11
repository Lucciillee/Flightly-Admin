namespace ProjectWebApp.ViewModels
{
    public class BookingDetailsVM
    {
        public int BookingId { get; set; }

        // User
        public string UserName { get; set; }
        public string Email { get; set; }

        // Booker info (new)

        public bool IsRegisteredUser { get; set; }
        public string AccountEmail { get; set; }  // Email of logged-in user who made the booking



        // Payment
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }

        // Flight
        public string FlightNumber { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string CabinClass { get; set; }

        // Booking
        public string Status { get; set; }

        // Return Flight (optional)
        public bool HasReturnFlight { get; set; }
        public string? ReturnFlightNumber { get; set; }
        public string? ReturnOriginCode { get; set; }
        public string? ReturnDestinationCode { get; set; }
        public DateTime? ReturnDepartureTime { get; set; }
        public DateTime? ReturnArrivalTime { get; set; }
    }
}
