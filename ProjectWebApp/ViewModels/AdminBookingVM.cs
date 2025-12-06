using System;

namespace ProjectWebApp.ViewModels
{
    public class AdminBookingVM
    {
        public int BookingId { get; set; }
        public string RefNumber => $"BK-{BookingId:D5}";

        public string UserName { get; set; }   // Registered user OR guest
        public string FlightInfo { get; set; } // Example: BAH→DXB (GF 008)

        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
