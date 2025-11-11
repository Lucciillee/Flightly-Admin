using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminDestinationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
