namespace ProjectWebApp.ViewModels
{
    public class BookingDetailsVM
    {
        public int BookingId { get; set; }

        // User
        public string UserName { get; set; }
        public string Email { get; set; }

        // Booker info (new)
      


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
    }
}
