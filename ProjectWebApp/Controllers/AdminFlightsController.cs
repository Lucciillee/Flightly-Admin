using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminFlightsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
