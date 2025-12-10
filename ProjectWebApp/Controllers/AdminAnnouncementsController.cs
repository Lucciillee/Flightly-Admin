using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;
using System.Security.Claims;

namespace ProjectWebApp.Controllers
{
    public class AdminAnnouncementsController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminAnnouncementsController(FlightlyDBContext context)
        {
            _context = context;
        }

        // LOAD PAGE
        public async Task<IActionResult> Index()
        {
            var vm = new AnnouncementVM
            {
                TotalAnnouncements = await _context.Announcements.CountAsync(),

                RecentAnnouncements = await _context.Announcements
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(10)
                    .Select(a => new AnnouncementItem
                    {
                        Title = a.Title,
                        Message = a.Message,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: SEND ANNOUNCEMENT
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Send(AnnouncementVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Message))
            {
                TempData["Error"] = "Please enter a title and message.";
                return RedirectToAction("Index");
            }

            // Get logged-in admin email
            var email = User.FindFirstValue(ClaimTypes.Email);

            int? adminId = null;

            if (!string.IsNullOrEmpty(email))
            {
                // Find the admin in database
                var admin = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (admin != null)
                {
                    adminId = admin.UserId;  // ⭐ Correct UserId
                }
            }

            var announcement = new Announcement
            {
                Title = model.Title,
                Message = model.Message,
                CreatedAt = DateTime.Now,
                CreatedBy = adminId,  // ⭐ Now filled correctly
                UserId = null         // sent to all users
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Announcement sent successfully!";
            return RedirectToAction("Index");
        }


    }
}
