using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;
using System.Security.Claims;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAnnouncementsController : Controller
    {
        private readonly FlightlyDBContext _context;
        private readonly EmailService _emailService;
        public AdminAnnouncementsController(FlightlyDBContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
        public async Task<IActionResult> Send(AnnouncementVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Message))
            {
                TempData["Error"] = "Please enter a title and message.";
                return RedirectToAction("Index");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);

            var admin = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.Email == email);

            var announcement = new Announcement
            {
                Title = model.Title,
                Message = model.Message,
                CreatedAt = DateTime.Now,
                CreatedBy = admin?.UserId,
                UserId = null
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync(); // ⭐ SAVE FIRST

            // ⭐ Get subscribed users
            var subscribedUsers = await _context.UserPreferences
                .Where(p => p.IsSubscribedToNewsletter)
                .Include(p => p.User)
                .Select(p => new
                {
                    p.User.Email,
                    p.User.FirstName
                })
                .ToListAsync();

            // ⭐ Send emails
            foreach (var user in subscribedUsers)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        $"📢 New Announcement: {model.Title}",
                        $@"
                    <p>Hi {user.FirstName},</p>
                    <p>{model.Message}</p>
                    <br/>
                    <p>— Flightly Team</p>
                "
                    );
                }
            }

            TempData["Success"] = "Announcement sent and emails delivered to subscribers!";
            return RedirectToAction("Index");
        }

    }
}
