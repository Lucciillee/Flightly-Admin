using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
