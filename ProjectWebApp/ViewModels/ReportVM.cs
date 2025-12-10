// ViewModels/ReportVM.cs
namespace ProjectWebApp.ViewModels
{
    public class ReportVM
    {
        public int? ReportTypeId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // For dropdown
        public List<ReportTypeItem> ReportTypes { get; set; }

        // For knowing which report shape to render
        public string SelectedReportCode { get; set; }

        // Preview data (only one of these will be filled at a time)
        public List<BookingReportRow> BookingRows { get; set; }          // TIME_RANGE
        public List<AirlineReportRow> AirlineRows { get; set; }          // AIRLINE
        public List<AirportReportRow> OriginRows { get; set; }           // ORIGIN
        public List<AirportReportRow> DestinationRows { get; set; }      // DESTINATION
        public List<CustomerReportRow> CustomerRows { get; set; }        // CUSTOMER
    }

    public class ReportTypeItem
    {
        public int ReportTypeId { get; set; }
        public string TypeName { get; set; }
        public string Code { get; set; }
    }

    // ---------- ROW DTOs ----------

    // TIME_RANGE
    public class BookingReportRow
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string PassengerName { get; set; }
        public string FlightNumber { get; set; }
        public string Route { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // AIRLINE
    public class AirlineReportRow
    {
        public string AirlineName { get; set; }
        public string AirlineCode { get; set; }
        public int FlightCount { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // ORIGIN / DESTINATION (same shape)
    public class AirportReportRow
    {
        public string AirportName { get; set; }
        public string AirportCode { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // CUSTOMER
    public class CustomerReportRow
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
