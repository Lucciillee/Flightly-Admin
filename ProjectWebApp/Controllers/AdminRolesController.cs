using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;

namespace ProjectWebApp.Controllers
{
    public class AdminRolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly FlightlyDBContext _context;
        public AdminRolesController(UserManager<IdentityUser> userManager, FlightlyDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeVM model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            // 1️⃣ Create identity user
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.TempPassword);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join("<br>", result.Errors.Select(e => e.Description));
                return RedirectToAction("Index");
            }

            // 2️⃣ Assign Identity Role
            await _userManager.AddToRoleAsync(user, model.Role);

            // 3️⃣ Save to FlightlyDB UserProfiles
            var profile = new UserProfile
            {
                IdentityUserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                RoleId = model.Role == "Admin" ? 1 : 2,
                CreatedAt = DateTime.Now,  // REQUIRED!
                IsDeleted = false          // Ensures active account
            };


            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{model.FirstName} {model.LastName} successfully created!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var profile = _context.UserProfiles.FirstOrDefault(x => x.IdentityUserId == id);
            if (profile != null)
            {
                profile.IsDeleted = true;
                _context.SaveChanges();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RestoreEmployee(string id)
        {
            var profile = _context.UserProfiles.FirstOrDefault(x => x.IdentityUserId == id);
            if (profile != null)
            {
                profile.IsDeleted = false;
                _context.SaveChanges();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.UserProfiles
                .Where(u => u.RoleId == 1 || u.RoleId == 2) // 1 = Admin, 2 = Sub-Admin
                .Select(u => new AdminUserVM
                {
                    IdentityUserId = u.IdentityUserId,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Role = u.RoleId == 1 ? "Admin" : "Sub-Admin",
                    IsDeleted = u.IsDeleted
                })
                .ToListAsync();

            return View(users);
        }

    }
}
