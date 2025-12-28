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
            if (!ModelState.IsValid)//If the user submitted invalid data (missing fields, wrong format), it rebuilds the view model (BuildIndexVM) and returns the same page with validation errors.
            {
                var vm = await BuildIndexVM(model);
                return View("Index", vm);
            }

            var user = new IdentityUser //A new IdentityUser is created with email, username, and confirmed email.
            {
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.TempPassword);//saves the user in the Identity system with a temporary password.

            if (!result.Succeeded)//If user creation fails (e.g., email already exists), it adds the errors to the ModelState, rebuilds the view model, and returns the same page with error messages.
            {
                ModelState.AddModelError("", string.Join(" ", result.Errors.Select(e => e.Description)));
                var vm = await BuildIndexVM(model);
                return View("Index", vm);
            }

            await _userManager.AddToRoleAsync(user, model.Role);//Assigns the specified role (Admin or Sub-Admin) to the newly created user.

            var profile = new UserProfile//A new UserProfile is created to store additional user details in the application database.
            {
                IdentityUserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                RoleId = model.Role == "Admin" ? 1 : 2,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.UserProfiles.Add(profile);//The UserProfile is added to the database context and saved.
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

        private async Task<AdminRolesIndexVM> BuildIndexVM(CreateEmployeeVM? form = null) //To avoid having to enter the data again if there are validation errors, the method accepts an optional CreateEmployeeVM parameter (form) that contains the submitted data.
        {
            return new AdminRolesIndexVM
            {
                CreateEmployee = form ?? new CreateEmployeeVM(),//If form is null (no data submitted yet), it creates a new empty form object.
                Users = await GetUsers()
            };
        }
        public async Task<IActionResult> Index()//prepares the form and the list of existing admin users to be displayed on the page.
        {
            var vm = await BuildIndexVM();//calling the BuildIndexVM method to get the view model. includes an empty CreateEmployeeVM for the form and a list of existing admin users.
            return View(vm);
        }

    }
}
