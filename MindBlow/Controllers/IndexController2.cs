using Microsoft.AspNetCore.Mvc;

namespace MindBlow.Controllers
{
    public class IndexController2 : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}