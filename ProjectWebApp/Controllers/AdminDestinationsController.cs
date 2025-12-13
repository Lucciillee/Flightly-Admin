using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using System;
using System.IO;
using System.Linq;

namespace ProjectWebApp.Controllers
{
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

            ViewBag.DeletedDestinations = deletedDestinations;

            return View(activeDestinations);
        }

        // ================= CREATE (GET) =================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        public IActionResult Create(string CityName, IFormFile ImageFile)
        {
            if (string.IsNullOrWhiteSpace(CityName) || ImageFile == null)
            {
                return View();
            }

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

            var destination = new FeaturedDestination
            {
                CityName = CityName,
                ImageUrl = "/uploads/destinations/" + fileName,
                CreatedAt = DateTime.Now
            };

            _context.FeaturedDestinations.Add(destination);
            _context.SaveChanges();

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





    }
}
