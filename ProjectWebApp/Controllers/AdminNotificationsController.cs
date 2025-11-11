using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminNotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
