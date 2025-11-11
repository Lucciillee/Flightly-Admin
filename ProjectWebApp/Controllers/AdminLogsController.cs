using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
