using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;

namespace ProjectWebApp.Controllers
{
    public class AdminBookingDetailsController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminBookingDetailsController(FlightlyDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            var b = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Guest)
                .Include(b => b.Payment).ThenInclude(p => p.PaymentStatus)
                .Include(b => b.Payment).ThenInclude(p => p.PaymentMethod)
                .Include(b => b.Flight).ThenInclude(f => f.OriginAirport)
                .Include(b => b.Flight).ThenInclude(f => f.DestinationAirport)
                .Include(b => b.BookingStatus)
                .Include(b => b.ClassType)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (b == null)
                return NotFound();

            // 👇 NEW LOGIC: Always show passenger info if exists
            var vm = new BookingDetailsVM
            {
                BookingId = b.BookingId,

                UserName = b.GuestId != null
                    ? $"{b.Guest.FirstName} {b.Guest.LastName}"
                    : $"{b.User.FirstName} {b.User.LastName}",

                Email = b.GuestId != null ? b.Guest.Email : b.User.Email,

                PaymentStatus = b.Payment.PaymentStatus.StatusName,
                PaymentMethod = b.Payment.PaymentMethod.MethodName,
                TotalAmount = b.TotalAmount,

                FlightNumber = b.Flight.FlightNumber,
                OriginCode = b.Flight.OriginAirport.Code,
                DestinationCode = b.Flight.DestinationAirport.Code,
                DepartureTime = b.Flight.DepartureTime,
                ArrivalTime = b.Flight.ArrivalTime,
                CabinClass = b.ClassType.ClassName,
                Status = b.BookingStatus.StatusName
            };

            return View(vm);
        }

        // 1) RESEND EMAIL
        public async Task<IActionResult> ResendEmail(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            _context.Logs.Add(new Log
            {
                UserId = booking.UserId,
                ActionType = "Resend Email",
                Description = $"Admin resent booking confirmation email for Booking #{id}.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Email has been resent (simulated).";
            return RedirectToAction("Index", new { id });
        }

        // 2) REFUND BOOKING
        public async Task<IActionResult> Refund(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payment)
                .ThenInclude(p => p.PaymentStatus)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            booking.Payment.PaymentStatusId = 2; // Refunded

            _context.Logs.Add(new Log
            {
                UserId = booking.UserId,
                ActionType = "Refund",
                Description = $"Admin refunded Booking #{id}.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking has been refunded.";
            return RedirectToAction("Index", new { id });
        }

        // 3) CANCEL BOOKING
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingStatus)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            booking.BookingStatusId = 2; // Cancelled

            _context.Logs.Add(new Log
            {
                UserId = booking.UserId,
                ActionType = "Cancel Booking",
                Description = $"Admin cancelled Booking #{id}.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking has been cancelled.";
            return RedirectToAction("Index", new { id });
        }
    }
}
