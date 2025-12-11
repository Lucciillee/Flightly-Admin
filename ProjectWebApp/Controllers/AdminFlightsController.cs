using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;
using System.Security.Claims;


namespace ProjectWebApp.Controllers
{
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
                .ToListAsync();

            return View(flights);
        }

        // GET: Create Flight
        public IActionResult Create()
        {
            var vm = new CreateFlightVM
            {
                Airlines = _context.Airlines
                    .Select(a => new SelectListItem { Value = a.AirlineId.ToString(), Text = a.AirlineName })
                    .ToList(),

                Airports = _context.Airports
                    .Select(a => new SelectListItem { Value = a.AirportId.ToString(), Text = a.Code })
                    .ToList(),

                Statuses = _context.FlightStatuses
                    .Select(s => new SelectListItem { Value = s.StatusId.ToString(), Text = s.StatusName })
                    .ToList()
            };

            return View(vm);
        }

        // POST: Create Flight
        [HttpPost]
        public async Task<IActionResult> Create(CreateFlightVM model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns
                model.Airlines = _context.Airlines.Select(a => new SelectListItem { Value = a.AirlineId.ToString(), Text = a.AirlineName }).ToList();
                model.Airports = _context.Airports.Select(a => new SelectListItem { Value = a.AirportId.ToString(), Text = a.Code }).ToList();
                model.Statuses = _context.FlightStatuses.Select(s => new SelectListItem { Value = s.StatusId.ToString(), Text = s.StatusName }).ToList();

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

            return RedirectToAction("Index");
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
            if (!ModelState.IsValid)
            {
                // Reload allowed statuses
                model.Statuses = _context.FlightStatuses
                    .Where(s => s.StatusName != "Cancelled")
                    .Select(s => new SelectListItem
                    {
                        Text = s.StatusName,
                        Value = s.StatusId.ToString()
                    })
                    .ToList();

                return View(model);
            }

            var flight = await _context.Flights.FindAsync(model.FlightId);
            if (flight == null)
                return NotFound();

            flight.FlightNumber = model.FlightNumber;
            flight.Aircraft = model.Aircraft;
            flight.DepartureTime = model.DepartureTime;
            flight.ArrivalTime = model.ArrivalTime;
            flight.BasePrice = model.BasePrice;
            flight.StatusId = model.StatusId;

            await _context.SaveChangesAsync();

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
