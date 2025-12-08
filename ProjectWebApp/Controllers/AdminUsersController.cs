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
                .Where(u => u.RoleId == 3);  // Show both active + blocked

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();
                users = users.Where(u =>
                    u.FirstName.ToLower().Contains(s) ||
                    u.LastName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s));
            }

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

        // -----------------------------
        // DELETE USER (Soft Delete)
        // -----------------------------
        public IActionResult Delete(int id)
        {
            var user = _context.UserProfiles.Find(id);
            if (user == null) return NotFound();

            user.IsDeleted = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
