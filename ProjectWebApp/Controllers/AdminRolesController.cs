using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminRolesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
