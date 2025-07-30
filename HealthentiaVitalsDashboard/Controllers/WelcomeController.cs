using Microsoft.AspNetCore.Mvc;

namespace HealthentiaVitalsDashboard.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}