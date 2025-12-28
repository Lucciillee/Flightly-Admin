using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> CreateEmployee(
     [Bind(Prefix = "CreateEmployee")] CreateEmployeeVM model)
        {
            if (!ModelState.IsValid)
            {
                var vm = await BuildIndexVM(model);
                return View("Index", vm);
            }

            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.TempPassword);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", string.Join(" ", result.Errors.Select(e => e.Description)));
                var vm = await BuildIndexVM(model);
                return View("Index", vm);
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            var profile = new UserProfile
            {
                IdentityUserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                RoleId = model.Role == "Admin" ? 1 : 2,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{model.FirstName} {model.LastName} successfully created!";
            return RedirectToAction("Index");
        }

        private async Task<List<AdminUserVM>> GetUsers()
        {
            return await _context.UserProfiles
                .Where(u => u.RoleId == 1 || u.RoleId == 2)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminUserVM
                {
                    IdentityUserId = u.IdentityUserId,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Role = u.RoleId == 1 ? "Admin" : "Sub-Admin",
                    IsDeleted = u.IsDeleted
                })
                .ToListAsync();
        }
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var profile = _context.UserProfiles.FirstOrDefault(x => x.IdentityUserId == id);
            if (profile != null)
            {
                profile.IsDeleted = true;
                _context.SaveChanges();
                TempData["Success"] = $"{profile.FirstName} {profile.LastName} has been blocked successfully!";

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

                TempData["Success"] = $"{profile.FirstName} {profile.LastName} has been restored successfully!";
            }

           

            return RedirectToAction("Index");
        }

        private async Task<AdminRolesIndexVM> BuildIndexVM(CreateEmployeeVM? form = null)
        {
            return new AdminRolesIndexVM
            {
                CreateEmployee = form ?? new CreateEmployeeVM(),
                Users = await GetUsers()
            };
        }
        public async Task<IActionResult> Index()
        {
            var vm = await BuildIndexVM();
            return View(vm);
        }

    }
}
