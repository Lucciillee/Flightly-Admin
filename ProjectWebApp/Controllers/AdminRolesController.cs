using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                RoleId = model.Role == "Admin" ? 1 : 2   // YOU DECIDE
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{model.FirstName} {model.LastName} successfully created!";
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
