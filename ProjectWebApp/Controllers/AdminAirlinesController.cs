using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;

namespace ProjectWebApp.Controllers
{
    public class AdminAirlinesController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminAirlinesController(FlightlyDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var airlines = await _context.Airlines
                .Include(a => a.Country)
                .Include(a => a.Status)
                .ToListAsync();

            return View(airlines);
        }



        public IActionResult Create()
        {
            var vm = new CreateAirlineVM
            {
                Countries = _context.Countries
                    .Select(c => new SelectListItem { Value = c.CountryId.ToString(), Text = c.CountryName })
                    .ToList(),

                Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem { Value = s.StatusId.ToString(), Text = s.StatusName })
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string AirlineName, int CountryId, string Code, int StatusId, IFormFile LogoFile)
        {
            string logoPath = null;

            // Handle logo upload
            if (LogoFile != null && LogoFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/airlines");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(fileStream);
                }

                logoPath = "/uploads/airlines/" + uniqueFileName;
            }

            // Save Airline
            var airline = new Airline
            {
                AirlineName = AirlineName,
                CountryId = CountryId,
                Code = Code,
                StatusId = StatusId,
                LogoUrl = logoPath,
                CreatedAt = DateTime.Now
            };

            _context.Airlines.Add(airline);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var airline = await _context.Airlines.FindAsync(id);
            if (airline == null)
                return NotFound();

            var vm = new EditAirlineVM
            {
                AirlineId = airline.AirlineId,
                AirlineName = airline.AirlineName,
                Code = airline.Code,
                CountryId = airline.CountryId,
                StatusId = airline.StatusId,
                ExistingLogoUrl = airline.LogoUrl,
                Countries = _context.Countries
                    .Select(c => new SelectListItem { Value = c.CountryId.ToString(), Text = c.CountryName })
                    .ToList(),
                Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem { Value = s.StatusId.ToString(), Text = s.StatusName })
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAirlineVM model, IFormFile NewLogo)
        {
            var airline = await _context.Airlines.FindAsync(model.AirlineId);
            if (airline == null)
                return NotFound();

            airline.AirlineName = model.AirlineName;
            airline.Code = model.Code;
            airline.CountryId = model.CountryId;
            airline.StatusId = model.StatusId;

            // LOGO UPDATE
            if (NewLogo != null && NewLogo.Length > 0)
            {
                // Delete old logo
                if (!string.IsNullOrEmpty(airline.LogoUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + airline.LogoUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/airlines");
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(NewLogo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await NewLogo.CopyToAsync(stream);
                }

                airline.LogoUrl = "/uploads/airlines/" + uniqueFileName;
            }

            airline.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            var airline = await _context.Airlines.FindAsync(id);
            if (airline == null)
                return NotFound();

            airline.StatusId = 2; // Inactive
            airline.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Activate(int id)
        {
            var airline = await _context.Airlines.FindAsync(id);
            if (airline == null)
                return NotFound();

            airline.StatusId = 1; // Active
            airline.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
