using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using System.Security.Claims;

namespace ProjectWebApp.Controllers
{
    public class AdminLogsController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminLogsController(FlightlyDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search = "", string actionType = "")
        {
            var logs = _context.Logs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .AsQueryable();

            // Search filter (email or description)
            if (!string.IsNullOrWhiteSpace(search))
            {
                logs = logs.Where(l =>
                    l.User.Email.Contains(search) ||
                    l.ActionType.Contains(search) ||
                    l.Description.Contains(search));
            }

            // Action filter
            if (!string.IsNullOrWhiteSpace(actionType))
            {
                logs = logs.Where(l => l.ActionType == actionType);
            }

            return View(await logs.ToListAsync());
        }

        // CSV Export
        public async Task<IActionResult> ExportCsv()
        {
            int? adminId = null;

            try
            {
                // 🧑‍💼 1️⃣ Identify admin
                var adminEmail = User.FindFirstValue(ClaimTypes.Email);
                var admin = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Email == adminEmail);

                adminId = admin?.UserId;

                // 📝 2️⃣ Log successful export
                _context.Logs.Add(new Log
                {
                    UserId = adminId,
                    ActionType = "Logs Exported",
                    Description = "Admin exported system logs as CSV",
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();

                // 📄 3️⃣ Generate CSV
                var logs = await _context.Logs
                    .Include(l => l.User)
                    .OrderByDescending(l => l.Timestamp)
                    .ToListAsync();

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("LogId,UserEmail,ActionType,Description,Timestamp");

                foreach (var log in logs)
                {
                    sb.AppendLine(
                        $"{log.LogId}," +
                        $"{log.User?.Email}," +
                        $"{log.ActionType}," +
                        $"\"{log.Description}\"," +
                        $"{log.Timestamp:yyyy-MM-dd HH:mm:ss}"
                    );
                }

                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                string fileName = $"Logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                // 🎉 4️⃣ Show success alert
                

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                // ❌ 5️⃣ Log failure in Logs table
                _context.Logs.Add(new Log
                {
                    UserId = adminId,
                    ActionType = "Logs Export Failed",
                    Description = "CSV export failed: " + ex.Message,
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();

                // ❗ 6️⃣ Show friendly error alert
                TempData["Error"] = "CSV export failed. Please try again.";

                return RedirectToAction("Index");
            }
        }


    }
}
