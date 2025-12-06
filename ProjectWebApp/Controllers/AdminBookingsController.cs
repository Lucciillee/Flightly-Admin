using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;

namespace ProjectWebApp.Controllers
{
    public class AdminBookingsController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminBookingsController(FlightlyDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int? statusId, DateTime? date)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Guest)
                .Include(b => b.Flight).ThenInclude(f => f.OriginAirport)
                .Include(b => b.Flight).ThenInclude(f => f.DestinationAirport)
                .Include(b => b.BookingStatus)
                .AsQueryable();

            //  Search by ref number or user name
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(b =>
                    b.BookingId.ToString().Contains(search) ||
                    (b.User != null &&
                        (b.User.FirstName + " " + b.User.LastName).ToLower().Contains(search)) ||
                    (b.Guest != null &&
                        (b.Guest.FirstName + " " + b.Guest.LastName).ToLower().Contains(search))
                );
            }

            //  Filter by status
            if (statusId.HasValue)
            {
                query = query.Where(b => b.BookingStatusId == statusId.Value);
            }

            //  Filter by date
            if (date.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date == date.Value.Date);
            }

            // Map to ViewModel
            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new AdminBookingVM
                {
                    BookingId = b.BookingId,
                    UserName = b.User != null
                        ? $"{b.User.FirstName} {b.User.LastName}"
                        : $"{b.Guest.FirstName} {b.Guest.LastName}",
                    FlightInfo = $"{b.Flight.OriginAirport.Code}→{b.Flight.DestinationAirport.Code} ({b.Flight.FlightNumber})",
                    BookingDate = b.BookingDate,
                    Status = b.BookingStatus.StatusName,
                    TotalAmount = b.TotalAmount
                })
                .ToListAsync();

            ViewBag.Statuses = await _context.BookingStatuses.ToListAsync();

            return View(bookings);
        }
    }
}