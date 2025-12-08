using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;

namespace ProjectWebApp.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminUsersController(FlightlyDBContext context)
        {
            _context = context;
        }

        // -----------------------------
        // LIST USERS (RoleId = 3)
        // -----------------------------
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

            return RedirectToAction("Index");
        }
    }
}
