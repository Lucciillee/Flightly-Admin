using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin,Sub-Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminDashboardController(FlightlyDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // =====================
            // TOP STATS
            // =====================
            ViewBag.TotalBookings = _context.Bookings.Count();

            ViewBag.ActiveFlights = _context.Flights.Count(f => f.StatusId == 1);

            ViewBag.RegisteredUsers = _context.UserProfiles
                .Count(u => u.RoleId == 3);

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            ViewBag.RevenueMTD = _context.Bookings
                .Where(b => b.BookingStatusId == 1 && b.BookingDate >= startOfMonth)
                .Sum(b => (decimal?)b.TotalAmount) ?? 0;

            // =====================
            // RECENT REVIEWS
            // =====================
               ViewBag.RecentReviews = _context.Reviews
              .Include(r => r.User)
              .Include(r => r.Booking)
                  .ThenInclude(b => b.Guest)
              .OrderByDescending(r => r.CreatedAt)
              .Take(5)
              .Select(r => new
              {
                  FullName =
                      r.User != null
                          ? r.User.FirstName + " " + r.User.LastName
                          : "Guest",

                  Email =
                      r.User != null
                          ? r.User.Email
                          : r.Booking.Guest != null
                              ? r.Booking.Guest.Email
                              : "—",

                  Rating = r.Rating,
                  Comment = r.Comment,
                  Date = r.CreatedAt
              })
              .ToList();


            // =====================
            // REVENUE TREND (LAST 6 MONTHS)
            // =====================
            var revenueRaw = _context.Bookings
                .Where(b => b.BookingStatusId == 1)
                .GroupBy(b => new
                {
                    b.BookingDate.Year,
                    b.BookingDate.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            ViewBag.RevenueLabels = revenueRaw
                .TakeLast(6)
                .Select(x => $"{x.Month}/{x.Year}")
                .ToList();

            ViewBag.RevenueData = revenueRaw
                .TakeLast(6)
                .Select(x => x.Total)
                .ToList();

            // =====================
            // USER GROWTH (LAST 6 MONTHS)
            // =====================
            var userGrowthRaw = _context.UserProfiles
                .Where(u => u.RoleId == 3)
                .GroupBy(u => new
                {
                    u.CreatedAt.Year,
                    u.CreatedAt.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            ViewBag.UserGrowthLabels = userGrowthRaw
                .TakeLast(6)
                .Select(x => new DateTime(x.Year, x.Month, 1).ToString("MMM"))
                .ToList();

            ViewBag.UserGrowthData = userGrowthRaw
                .TakeLast(6)
                .Select(x => x.Count)
                .ToList();

            // =====================
            // TOP ROUTES (DONUT)
            // =====================
            var topRoutesRaw = _context.Bookings
                .Include(b => b.Flight)
                    .ThenInclude(f => f.OriginAirport)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.DestinationAirport)
                .GroupBy(b => new
                {
                    From = b.Flight.OriginAirport.Code,
                    To = b.Flight.DestinationAirport.Code
                })
                .Select(g => new
                {
                    Route = $"{g.Key.From}→{g.Key.To}",
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            ViewBag.TopRoutesLabels = topRoutesRaw.Select(x => x.Route).ToList();
            ViewBag.TopRoutesData = topRoutesRaw.Select(x => x.Count).ToList();

            // =====================
            // TOP PERFORMING AIRLINES (WITH REVENUE SHARE)
            // =====================

            // Calculate the total revenue for the current month
            var totalRevenueThisMonth = _context.Bookings
                .Where(b => // Filter bookings to include only those that are confirmed (BookingStatusId == 1)
                       b.BookingDate >= startOfMonth // And bookings that started from the beginning of the current month
                   )
                .Sum(b => (decimal?)b.TotalAmount) ?? 0; // Sum up the total amount of these bookings, and if there are none, default to 0

            // Get the top airlines for the current month
            var topAirlinesRaw = _context.Bookings
                .Where(b => // Similarly, filter bookings to include only confirmed ones and from the current month
                       b.BookingStatusId == 1 &&
                       b.BookingDate >= startOfMonth
                   )
                .Include(b => b.Flight) // Include the flight information for each booking
                    .ThenInclude(f => f.Airline) // Then include the airline information for each flight
                .GroupBy(b => b.Flight.Airline.AirlineName) // Group the bookings by airline name
                .Select(g => new // For each group, create a new object
                {
                    Airline = g.Key, // The airline name is the key of the group
                    BookingsThisMonth = g.Count(), // Count how many bookings are in this group
                    Revenue = g.Sum(x => x.TotalAmount), // Sum up the total revenue for this airline
                    RevenueShare = totalRevenueThisMonth == 0
                        ? 0
                        : (g.Sum(x => x.TotalAmount) / totalRevenueThisMonth) * 100 // Calculate the percentage of total revenue this airline contributes
                })
                .OrderByDescending(x => x.Revenue) // Order the airlines by revenue in descending order
                .Take(5) // Take the top 5 airlines
                .ToList(); // Convert the result to a list

            // Pass the top airlines data to the view
            ViewBag.TopAirlines = topAirlinesRaw;

            // Return the view to render the dashboard
            return View();
        }
    }
}
