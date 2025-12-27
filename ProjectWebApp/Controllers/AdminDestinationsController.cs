using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin,Sub-Admin")]
    public class AdminDestinationsController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminDestinationsController(FlightlyDBContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public IActionResult Index()
        {
            var activeDestinations = _context.FeaturedDestinations
                .Where(d => d.DeletedAt == null)
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            var deletedDestinations = _context.FeaturedDestinations
                .Where(d => d.DeletedAt != null)
                .OrderByDescending(d => d.DeletedAt)
                .ToList();

           var popup = _context.TourismPopups
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefault();

            ViewBag.TourismPopup = popup;
            ViewBag.DeletedDestinations = deletedDestinations;

            return View(activeDestinations);
        }

        // ================= CREATE (GET) ================= Display the create form 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ================= CREATE (POST) =================//after submitting the form
        [HttpPost]
        public IActionResult Create(string CityName, IFormFile ImageFile)
        {
            // 🔵 Normalize city name
            CityName = CityName?.Trim();

            if (string.IsNullOrWhiteSpace(CityName) || ImageFile == null)
            {
                ModelState.AddModelError("", "City name and image are required.");
                return View();
            }

            //Convert city name to Title case for polished UI
            CityName = CultureInfo.CurrentCulture.TextInfo
    .ToTitleCase(CityName.ToLower());

            // 🔴 CHECK: City already exists (case-insensitive)
            bool cityExists = _context.FeaturedDestinations
                .Any(d => d.CityName.ToLower() == CityName.ToLower());

            if (cityExists)
            {
                ModelState.AddModelError(
                    "CityName",
                    "This city already exists."
                );
                return View();
            }
            //define the folder in which the image will be saved
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/destinations"
            );

            //create folder if it does not exist alredy
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            //generate a unique file name for the uploaded image
            var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            //Save the uploaded image to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

            //Get the id of the currently logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Create a new destination entity or record
            var destination = new FeaturedDestination
            {
                CityName = CityName,
                ImageUrl = "/uploads/destinations/" + fileName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
               

            };

            //Add new destination to the database and save changes
            _context.FeaturedDestinations.Add(destination);
            _context.SaveChanges();

            TempData["Success"] = "Destination added successfully.";
            return RedirectToAction("Index");
        }

        // ================= DELETE =================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var destination = _context.FeaturedDestinations
                .FirstOrDefault(d => d.DestinationId == id);

            if (destination == null)
                return NotFound();

            destination.DeletedAt = DateTime.Now;
            destination.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ================= RESTORE =================
        [HttpPost]
        public IActionResult Restore(int id)
        {
            var destination = _context.FeaturedDestinations
                .FirstOrDefault(d => d.DestinationId == id);

            if (destination == null)
                return NotFound();

            destination.DeletedAt = null;
            destination.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(int id, string CityName, IFormFile? ImageFile)
        {
            if (string.IsNullOrWhiteSpace(CityName))
            {
                return RedirectToAction("Index");
            }

            var destination = _context.FeaturedDestinations
                .FirstOrDefault(d => d.DestinationId == id);

            if (destination == null)
            {
                return NotFound();
            }

            // Always update city name
            destination.CityName = CityName;
            destination.UpdatedAt = DateTime.Now;

            // Update image ONLY if a new one was uploaded
            if (ImageFile != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/destinations"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                destination.ImageUrl = "/uploads/destinations/" + fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateTourismPopup(string CityName, string PopupText, IFormFile? ImageFile)
        {
            // Always get the SAME popup (the first one ever created)
            var popup = _context.TourismPopups
                .OrderBy(p => p.CreatedAt)
                .FirstOrDefault();

            if (popup == null)
            {
                popup = new TourismPopup
                {
                    CreatedAt = DateTime.Now
                };
                _context.TourismPopups.Add(popup);
            }

            popup.PlaceName = CityName;
            popup.PopupText = PopupText;
            popup.UpdatedAt = DateTime.Now;

            if (ImageFile != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/popup"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                //generate a unique file name for the uploaded image
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                //combine the folder path and the unique file name to get the full file path where the image will be saved
                var filePath = Path.Combine(uploadsFolder, fileName);

                //Save the uploaded image to the server, This part creates a new file stream at the specified file path and then copies the uploaded image into that stream. Essentially, this is the step where the image is actually saved to the server’s file system
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                // Update the ImageUrl property of the popup to point to the new image location
                popup.ImageUrl = "/uploads/popup/" + fileName;
            }

            _context.SaveChanges();
            TempData["Success"] = "Tourism popup updated successfully!";
            return RedirectToAction("Index");
        }




    }
}
