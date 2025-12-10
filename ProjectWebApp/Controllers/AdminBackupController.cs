using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using System.Security.Claims;

namespace ProjectWebApp.Controllers
{
    public class AdminBackupController : Controller
    {
        private readonly FlightlyDBContext _context;
        private readonly IConfiguration _config;

        public AdminBackupController(FlightlyDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var lastBackup = await _context.BackupHistories
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefaultAsync();

            ViewBag.LastBackup = lastBackup?.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                                 ?? "No backups yet";

            ViewBag.Backups = await _context.BackupHistories
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> BackupNow()
        {
            int? adminId = null;  // ⭐ Declare here so it can be used in catch

            try
            {
                // Get logged-in admin user ID
                var adminEmail = User.FindFirstValue(ClaimTypes.Email);
                var admin = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Email == adminEmail);

                adminId = admin?.UserId;   // ⭐ Assign here

                // Backup folder
                string folder = @"C:\FlightlyBackups\";

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // File name
                string fileName = $"FlightlyDB_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string fullPath = Path.Combine(folder, fileName);

                // SQL backup
                string sql = $@"
            BACKUP DATABASE [FlightlyDB]
            TO DISK = '{fullPath}'
            WITH INIT, STATS = 5;
        ";

                using (var conn = new SqlConnection(_config.GetConnectionString("FlightlyDB")))
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Save backup history
                var history = new BackupHistory
                {
                    FileName = fileName,
                    FilePath = fullPath,
                    BackupType = "Full",
                    BackupStatus = "Success",
                    CreatedAt = DateTime.Now,
                    UserId = adminId
                };

                _context.BackupHistories.Add(history);
                await _context.SaveChangesAsync();

                // Log success
                _context.Logs.Add(new Log
                {
                    UserId = adminId,
                    ActionType = "Backup",
                    Description = $"Database backup created: {fileName}",
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();

                TempData["Success"] = "Backup completed successfully.";
            }
            catch (Exception ex)
            {
                // Log backup failure
                _context.Logs.Add(new Log
                {
                    UserId = adminId,   // ⭐ NOW we correctly log the admin who triggered the failure
                    ActionType = "Backup Failed",
                    Description = "Backup failed: " + ex.Message,
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();
                TempData["Error"] = "Backup failed: " + ex.Message;
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Restore(int backupId)
        {
            // ⭐ FIX: declare adminId BEFORE the try
            int? adminId = null;

            try
            {
                // Find selected backup
                var backup = await _context.BackupHistories.FindAsync(backupId);
                if (backup == null)
                {
                    TempData["Error"] = "Backup file not found.";
                    return RedirectToAction("Index");
                }

                string filePath = backup.FilePath;

                // Get admin user
                var adminEmail = User.FindFirstValue(ClaimTypes.Email);
                var admin = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.Email == adminEmail);

                adminId = admin?.UserId;   // ⭐ Assign here

                // SQL restore
                string sql = $@"
            ALTER DATABASE [FlightlyDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [FlightlyDB]
            FROM DISK = '{filePath}'
            WITH REPLACE;
            ALTER DATABASE [FlightlyDB] SET MULTI_USER;
        ";

                using (var conn = new SqlConnection(_config.GetConnectionString("FlightlyDB")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Save restore info
                backup.RestoreStatus = "Success";
                backup.RestoreTimestamp = DateTime.Now;
                await _context.SaveChangesAsync();

                // Log restore
                _context.Logs.Add(new Log
                {
                    UserId = adminId,
                    ActionType = "Restore",
                    Description = $"Database restored using file: {backup.FileName}",
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();

                TempData["Success"] = "Database restored successfully.";
            }
            catch (Exception ex)
            {
                // Log restore failure
                _context.Logs.Add(new Log
                {
                    UserId = adminId,
                    ActionType = "Restore Failed",
                    Description = "Restore failed: " + ex.Message,
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();

                TempData["Error"] = "Restore failed: " + ex.Message;
            }

            return RedirectToAction("Index");
        }


    }
}
