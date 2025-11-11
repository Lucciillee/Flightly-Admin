using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminBookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
