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
public async Task<IActionResult> Create(CreateAirlineVM model)
{
            // Normalize airline code (prevents lh / LH duplicates)
            model.Code = model.Code?.Trim().ToUpper();

            // 🔵 ADDED: Normalize airline name
            model.AirlineName = model.AirlineName?.Trim();

            if (!ModelState.IsValid)
    {
        // Refill dropdowns (VERY IMPORTANT)
        model.Countries = _context.Countries
            .Select(c => new SelectListItem
            {
                Value = c.CountryId.ToString(),
                Text = c.CountryName
            }).ToList();

        model.Statuses = _context.AirlineStatuses
            .Select(s => new SelectListItem
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusName
            }).ToList();

        return View(model);

    }
            // 🔴 CHECK: Airline code already exists
            bool codeExists = await _context.Airlines
                .AnyAsync(a => a.Code == model.Code);

            if (codeExists)
            {
                ModelState.AddModelError(
                    "Code",
                    "This airline code already exists."
                );

                model.Countries = _context.Countries
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                model.Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList();

                return View(model);
            }
            // 🔵 ADDED: Airline NAME duplicate check (soft uniqueness)
            bool nameExists = await _context.Airlines
                .AnyAsync(a => a.AirlineName.ToLower() == model.AirlineName.ToLower());

            if (nameExists)
            {
                ModelState.AddModelError(
                    "AirlineName",
                    "An airline with this name already exists."
                );

                model.Countries = _context.Countries
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                model.Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList();

                return View(model);
            }

            // ✅ ADD THIS BLOCK RIGHT HERE
            var email = User.FindFirstValue(ClaimTypes.Email);

            int? adminId = null;

            if (!string.IsNullOrEmpty(email))
            {
                var admin = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (admin != null)
                    adminId = admin.UserId;
            }

            string logoPath = null;

    if (model.LogoFile != null && model.LogoFile.Length > 0)
    {
        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot/uploads/airlines"
        );

        Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid() + Path.GetExtension(model.LogoFile.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await model.LogoFile.CopyToAsync(stream);

        logoPath = "/uploads/airlines/" + fileName;
    }

            var airline = new Airline
            {
                AirlineName = model.AirlineName,
                CountryId = model.CountryId,
                Code = model.Code,
                StatusId = model.StatusId,
                LogoUrl = logoPath,
                UserId = adminId, // ⭐ THIS WAS MISSING
                CreatedAt = DateTime.Now
            };

            _context.Airlines.Add(airline);
            //NEW
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // 🔒 DB-level safety (unique constraint)
                ModelState.AddModelError(
                    "Code",
                    "This airline code already exists."
                );

                model.Countries = _context.Countries
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                model.Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList();

                return View(model);
            }

            TempData["Success"] = "Airline added successfully.";

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
            // 🔵 Normalize inputs
            model.Code = model.Code?.Trim().ToUpper();
            model.AirlineName = model.AirlineName?.Trim();

            if (!ModelState.IsValid)
            {
                model.Countries = _context.Countries
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                model.Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList();

                return View(model);
            }

            // 🔴 CHECK: Duplicate airline CODE (exclude current airline)
            bool codeExists = await _context.Airlines.AnyAsync(a =>
                a.Code == model.Code &&
                a.AirlineId != model.AirlineId
            );

            if (codeExists)
            {
                ModelState.AddModelError("Code", "This airline code already exists.");

                model.Countries = _context.Countries
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                model.Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList();

                return View(model);
            }

            // 🔴 CHECK: Duplicate airline NAME (exclude current airline)
            bool nameExists = await _context.Airlines.AnyAsync(a =>
                a.AirlineName.ToLower() == model.AirlineName.ToLower() &&
                a.AirlineId != model.AirlineId
            );

            if (nameExists)
            {
                ModelState.AddModelError(
                    "AirlineName",
                    "An airline with this name already exists."
                );

                model.Countries = _context.Countries
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                model.Statuses = _context.AirlineStatuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId.ToString(),
                        Text = s.StatusName
                    }).ToList();

                return View(model);
            }

            var airline = await _context.Airlines.FindAsync(model.AirlineId);
            if (airline == null)
                return NotFound();

            airline.AirlineName = model.AirlineName;
            airline.Code = model.Code;
            airline.CountryId = model.CountryId;
            airline.StatusId = model.StatusId;

            // 🖼️ LOGO UPDATE
            if (NewLogo != null && NewLogo.Length > 0)
            {
                if (!string.IsNullOrEmpty(airline.LogoUrl))
                {
                    var oldPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot" + airline.LogoUrl
                    );

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/airlines"
                );

                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(NewLogo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await NewLogo.CopyToAsync(stream);

                airline.LogoUrl = "/uploads/airlines/" + fileName;
            }

            airline.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Airline updated successfully.";
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
