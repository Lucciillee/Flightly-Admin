using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin,Sub-Admin")]
    public class AdminBookingDetailsController : Controller
    {
        private readonly FlightlyDBContext _context;
        private readonly EmailService _emailService;
        public AdminBookingDetailsController(FlightlyDBContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;

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
                .Include(b => b.ReturnFlight).ThenInclude(f => f.OriginAirport)
                .Include(b => b.ReturnFlight).ThenInclude(f => f.DestinationAirport)
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
                IsRegisteredUser = b.UserId != null,//NEW
                AccountEmail = b.UserId != null ? b.User.Email : null,   // NEW

                // Booked by (account holder)

                PaymentStatus = b.Payment.PaymentStatus.StatusName,
                PaymentMethod = b.Payment.PaymentMethod.MethodName,
                TotalAmount = b.TotalAmount,

                FlightNumber = b.Flight.FlightNumber,
                OriginCode = b.Flight.OriginAirport.Code,
                DestinationCode = b.Flight.DestinationAirport.Code,
                DepartureTime = b.Flight.DepartureTime,
                ArrivalTime = b.Flight.ArrivalTime,
                CabinClass = b.ClassType.ClassName,
                Status = b.BookingStatus.StatusName,

                HasReturnFlight = b.ReturnFlightId != null,

                ReturnFlightNumber = b.ReturnFlightId != null ? b.ReturnFlight.FlightNumber : null,
                ReturnOriginCode = b.ReturnFlightId != null ? b.ReturnFlight.OriginAirport.Code : null,
                ReturnDestinationCode = b.ReturnFlightId != null ? b.ReturnFlight.DestinationAirport.Code : null,
                ReturnDepartureTime = b.ReturnFlightId != null ? b.ReturnFlight.DepartureTime : null,
                ReturnArrivalTime = b.ReturnFlightId != null ? b.ReturnFlight.ArrivalTime : null,

            };

            return View(vm);
        }

        // 1) RESEND EMAIL
        // 1) RESEND EMAIL
        public async Task<IActionResult> ResendEmail(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            // ------------------ LOAD FLIGHTS ------------------
            var outbound = await _context.Flights
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .FirstAsync(f => f.FlightId == booking.FlightId);

            Flight? returnFlight = null;
            if (booking.ReturnFlightId.HasValue)
            {
                returnFlight = await _context.Flights
                    .Include(f => f.OriginAirport)
                    .Include(f => f.DestinationAirport)
                    .FirstOrDefaultAsync(f => f.FlightId == booking.ReturnFlightId.Value);
            }

            // ------------------ EMAIL HTML ------------------
            string emailHtml = $@"
<h2>Flightly Booking Confirmation</h2>
<p>Thank you <strong>{booking.Guest.FirstName}</strong>, your booking is confirmed!</p>

<h3>Booking Reference: <strong>{booking.BookingId}</strong></h3>

<h4>Outbound Flight</h4>
<p>
    <strong>From:</strong> {outbound.OriginAirport.AirportName} ({outbound.OriginAirport.City})<br/>
    <strong>To:</strong> {outbound.DestinationAirport.AirportName} ({outbound.DestinationAirport.City})<br/>
    <strong>Date:</strong> {outbound.DepartureTime:dddd, MMM dd yyyy HH:mm}<br/>
</p>
";

            if (returnFlight != null)
            {
                emailHtml += $@"
<h4>Return Flight</h4>
<p>
    <strong>From:</strong> {returnFlight.OriginAirport.AirportName} ({returnFlight.OriginAirport.City})<br/>
    <strong>To:</strong> {returnFlight.DestinationAirport.AirportName} ({returnFlight.DestinationAirport.City})<br/>
    <strong>Date:</strong> {returnFlight.DepartureTime:dddd, MMM dd yyyy HH:mm}<br/>
</p>
";
            }

            emailHtml += $@"
<h3>Total Paid: BHD {booking.TotalAmount:0.000}</h3>
<hr/>
<p>You can later create an account and claim your booking using your email and booking reference.</p>
";

            // ------------------ SEND EMAIL ------------------
            await _emailService.SendEmailAsync(
                booking.Guest.Email,
                "Your Flightly Booking Confirmation",
                emailHtml
            );

            // ------------------ LOG (KEEPED) ------------------
            _context.Logs.Add(new Log
            {
                UserId = booking.UserId,
                ActionType = "Resend Email",
                Description = $"Admin resent booking confirmation email for Booking #{id}.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking confirmation email has been resent successfully.";
            return RedirectToAction("Index", new { id });
        }


        // 2) REFUND BOOKING
        // 2) REFUND BOOKING
        public async Task<IActionResult> Refund(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Payment)
                .ThenInclude(p => p.PaymentStatus)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            // ------------------ UPDATE PAYMENT ------------------
            booking.Payment.PaymentStatusId = 2; // Refunded

            // ------------------ REFUND EMAIL ------------------
            string refundEmail = $@"
<h2>Flightly Refund Confirmation</h2>

<p>Your refund has been successfully processed.</p>

<h3>Booking Reference: {booking.BookingId}</h3>
<h3>Refunded Amount: BHD {booking.TotalAmount:0.000}</h3>

<p>If you have any questions, please contact Flightly support.</p>
";

            await _emailService.SendEmailAsync(
                booking.Guest.Email,
                "Flightly Refund Confirmation",
                refundEmail
            );

            // ------------------ LOG (KEEPED) ------------------
            _context.Logs.Add(new Log
            {
                UserId = booking.UserId,
                ActionType = "Refund",
                Description = $"Admin refunded Booking #{id}.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking has been refunded and refund email sent.";
            return RedirectToAction("Index", new { id });
        }


        // 3) CANCEL BOOKING
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings
     .Include(b => b.Guest)
     .Include(b => b.User)
     .Include(b => b.Flight)
         .ThenInclude(f => f.OriginAirport)
     .Include(b => b.Flight)
         .ThenInclude(f => f.DestinationAirport)
     .FirstOrDefaultAsync(b => b.BookingId == id);


            if (booking == null)
                return NotFound();

            booking.BookingStatusId = 2; // Cancelled

            string cancelEmail = $@"
            <h2>Flightly Booking Cancelled</h2>

            <p>Dear <strong>{booking.Guest.FirstName}</strong>,</p>

            <p>Your booking has been <strong>cancelled</strong>.</p>

            <h3>Booking Reference: {booking.BookingId}</h3>

            <p>
                <strong>Flight:</strong> {booking.Flight.FlightNumber}<br/>
                <strong>Route:</strong> {booking.Flight.OriginAirport.Code} → {booking.Flight.DestinationAirport.Code}
            </p>

            <p>If you have already been charged, a refund will be processed according to our policy.</p>

            <p>Thank you for choosing Flightly.</p>
            ";

                        await _emailService.SendEmailAsync(
                booking.Guest.Email,
                "Your Flightly Booking Has Been Cancelled",
                cancelEmail
            );

            _context.Logs.Add(new Log
            {
                UserId = booking.UserId,
                ActionType = "Cancel Booking",
                Description = $"Admin cancelled Booking #{id}.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking has been cancelled and cancellation email sent.";
            return RedirectToAction("Index", new { id });
        }


    }
}
