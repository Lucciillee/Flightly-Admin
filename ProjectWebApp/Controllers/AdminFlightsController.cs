using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;
using System.Security.Claims;


namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin,Sub-Admin")]
    public class AdminFlightsController : Controller
    {

        private readonly FlightlyDBContext _context;

        public AdminFlightsController(FlightlyDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var flights = await _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .Include(f => f.Status)
                .OrderByDescending(f => f.CreatedAt) // 👈 THIS LINE
                .ToListAsync();

            return View(flights);
        }

        // GET: Create Flight
        public IActionResult Create()
        {
            var now = DateTime.Now;

            var departure = new DateTime(
                now.Year,
                now.Month,
                now.Day,
                now.Hour + 1,
                now.Minute,
                0 // seconds
            );

            var arrival = departure.AddHours(2);

            var vm = new CreateFlightVM
            {
                DepartureTime = departure,
                ArrivalTime = arrival,

                Airlines = _context.Airlines
                    .Select(a => new SelectListItem
                    {
                        Value = a.AirlineId.ToString(),
                        Text = a.AirlineName
                    }).ToList(),

                Airports = _context.Airports
                    .Select(a => new SelectListItem
                    {
                        Value = a.AirportId.ToString(),
                        Text = a.Code
                    }).ToList(),

                Statuses = _context.FlightStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList()
            };

            return View(vm);
        }




        // POST: Create Flight
        [HttpPost]
        public async Task<IActionResult> Create(CreateFlightVM model)
        {
            // Normalize flight number
            model.FlightNumber = model.FlightNumber?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                ReloadDropdowns(model);
                return View(model);
            }

            // 🔴 NEW CHECK: Origin and Destination cannot be the same
            if (model.OriginAirportId == model.DestinationAirportId)
            {
                ModelState.AddModelError(
                    "DestinationAirportId",
                    "Origin and destination airports must be different."
                );

                ReloadDropdowns(model);
                return View(model);
            }

            // 🔴 CHECK: Flight number already exists
            bool flightExists = await _context.Flights
                .AnyAsync(f => f.FlightNumber == model.FlightNumber);

            if (flightExists)
            {
                ModelState.AddModelError(
                    "FlightNumber",
                    "This flight number is already taken."
                );

                ReloadDropdowns(model);
                return View(model);
            }

            // ✅ Get logged-in admin
            var email = User.FindFirstValue(ClaimTypes.Email);
            int? adminId = null;

            if (!string.IsNullOrEmpty(email))
            {
                var admin = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (admin != null)
                    adminId = admin.UserId;
            }

            var flight = new Flight
            {
                AirlineId = model.AirlineId,
                FlightNumber = model.FlightNumber,
                Aircraft = model.Aircraft,
                OriginAirportId = model.OriginAirportId,
                DestinationAirportId = model.DestinationAirportId,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime,
                BasePrice = model.BasePrice,
                StatusId = model.StatusId,
                UserId = adminId,
                CreatedAt = DateTime.Now
            };

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Flight added successfully.";
            return RedirectToAction("Index");
        }

        private void ReloadDropdowns(CreateFlightVM model)
        {
            model.Airlines = _context.Airlines
                .Select(a => new SelectListItem
                {
                    Value = a.AirlineId.ToString(),
                    Text = a.AirlineName
                }).ToList();

            model.Airports = _context.Airports
                .Select(a => new SelectListItem
                {
                    Value = a.AirportId.ToString(),
                    Text = a.Code
                }).ToList();

            model.Statuses = _context.FlightStatuses
                .Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.StatusName
                }).ToList();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .Include(f => f.Status)
                .FirstOrDefaultAsync(f => f.FlightId == id);

            if (flight == null)
                return NotFound();

            var vm = new EditFlightVM
            {
                FlightId = flight.FlightId,
                FlightNumber = flight.FlightNumber,
                Aircraft = flight.Aircraft,
                BasePrice = flight.BasePrice,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                StatusId = flight.StatusId,

                AirlineName = flight.Airline.AirlineName,
                OriginCode = flight.OriginAirport.Code,
                DestinationCode = flight.DestinationAirport.Code,

                // Only allow "On Time" and "Delayed" inside Edit screen
                Statuses = _context.FlightStatuses
            .Where(s => s.StatusName != "Cancelled")
            .Select(s => new SelectListItem
            {
                Text = s.StatusName,
                Value = s.StatusId.ToString()
            })
            .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditFlightVM model)
        {
            // Normalize
            model.FlightNumber = model.FlightNumber?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                // 🔁 Reload statuses
                model.Statuses = _context.FlightStatuses
                    .Where(s => s.StatusName != "Cancelled")
                    .Select(s => new SelectListItem
                    {
                        Text = s.StatusName,
                        Value = s.StatusId.ToString()
                    })
                    .ToList();

                // 🔁 RELOAD NON-POSTED DATA
                var flight = await _context.Flights
                    .Include(f => f.Airline)
                    .Include(f => f.OriginAirport)
                    .Include(f => f.DestinationAirport)
                    .FirstOrDefaultAsync(f => f.FlightId == model.FlightId);

                if (flight != null)
                {
                    model.AirlineName = flight.Airline.AirlineName;
                    model.OriginCode = flight.OriginAirport.Code;
                    model.DestinationCode = flight.DestinationAirport.Code;
                }

                return View(model);
            }

            // 🔴 Duplicate flight number check
            bool exists = await _context.Flights.AnyAsync(f =>
                f.FlightNumber == model.FlightNumber &&
                f.FlightId != model.FlightId
            );

            if (exists)
            {
                ModelState.AddModelError("FlightNumber", "This flight number already exists.");

                model.Statuses = _context.FlightStatuses
                    .Where(s => s.StatusName != "Cancelled")
                    .Select(s => new SelectListItem
                    {
                        Text = s.StatusName,
                        Value = s.StatusId.ToString()
                    })
                    .ToList();

                // 🔁 RELOAD NON-POSTED DATA AGAIN
                var flight = await _context.Flights
                    .Include(f => f.Airline)
                    .Include(f => f.OriginAirport)
                    .Include(f => f.DestinationAirport)
                    .FirstOrDefaultAsync(f => f.FlightId == model.FlightId);

                if (flight != null)
                {
                    model.AirlineName = flight.Airline.AirlineName;
                    model.OriginCode = flight.OriginAirport.Code;
                    model.DestinationCode = flight.DestinationAirport.Code;
                }

                return View(model);
            }

            var entity = await _context.Flights.FindAsync(model.FlightId);
            if (entity == null)
                return NotFound();

            entity.FlightNumber = model.FlightNumber;
            entity.Aircraft = model.Aircraft;
            entity.DepartureTime = model.DepartureTime;
            entity.ArrivalTime = model.ArrivalTime;
            entity.BasePrice = model.BasePrice;
            entity.StatusId = model.StatusId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Flight updated successfully.";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
                return NotFound();

            // Cancelled = StatusId 3 (from your DB)
            flight.StatusId = 3;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
