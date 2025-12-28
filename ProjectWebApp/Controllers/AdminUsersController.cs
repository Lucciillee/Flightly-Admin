using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin,Sub-Admin")]
    public class AdminUsersController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminUsersController(FlightlyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index(string search)
        {
            var users = _context.UserProfiles
                .Where(u => u.RoleId == 3); // Show ALL: active + blocked

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();
                users = users.Where(u =>
                    u.FirstName.ToLower().Contains(s) ||
                    u.LastName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s));
            }
            // Order users by most recent first
            users = users.OrderByDescending(u => u.CreatedAt);
            // --- Monthly Chart Data ---
            var monthlyCounts = _context.UserProfiles
                .Where(u => u.RoleId == 3)
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            ViewBag.MonthLabels = monthlyCounts
                .Select(m => new DateTime(m.Year, m.Month, 1).ToString("MMM"))
                .ToArray();

            ViewBag.MonthCounts = monthlyCounts
                .Select(m => m.Count)
                .ToArray();

            return View(users.ToList());
        }

        // -----------------------------
        // BLOCK USER
        // -----------------------------
        public IActionResult Block(int id)
        {
            var user = _context.UserProfiles.Find(id);
            if (user == null) return NotFound();

            user.IsDeleted = true;
            _context.SaveChanges();

            TempData["Success"] = $"{user.FirstName} {user.LastName} has been blocked successfully.";

            return RedirectToAction("Index");
        }

        // -----------------------------
        // UNBLOCK USER
        // -----------------------------
        public IActionResult Unblock(int id)
        {
            var user = _context.UserProfiles.Find(id);
            if (user == null) return NotFound();

            user.IsDeleted = false;
            _context.SaveChanges();

            TempData["Success"] = $"{user.FirstName} {user.LastName} has been unblocked successfully.";

            return RedirectToAction("Index");
        }

        // -----------------------------
        // USER DETAILS
        // -----------------------------
        public IActionResult Details(int id)
        {
            var user = _context.UserProfiles
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
                return NotFound();

            // Optional: log view details
            _context.Logs.Add(new Log
            {
                UserId = null, // admin id optional
                ActionType = "Viewed User Details",
                Description = $"Viewed details for user {user.Email}",
                Timestamp = DateTime.Now
            });

            _context.SaveChanges();

            return View(user);
        }
    }
}
