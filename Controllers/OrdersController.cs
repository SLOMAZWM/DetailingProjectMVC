using Microsoft.AspNetCore.Mvc;

namespace ProjektLABDetailing.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
