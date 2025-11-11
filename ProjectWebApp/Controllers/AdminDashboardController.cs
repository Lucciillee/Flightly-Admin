using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
