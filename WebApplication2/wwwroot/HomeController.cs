using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.wwwroot
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
