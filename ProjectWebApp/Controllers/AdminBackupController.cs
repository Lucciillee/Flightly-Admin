using Microsoft.AspNetCore.Mvc;

namespace ProjectWebApp.Controllers
{
    public class AdminBackupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
