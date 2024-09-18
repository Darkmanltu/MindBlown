using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace MindBlow.Controllers
{
    [Route("api/[controller]")]
    public class IndexController : Controller
    {
        [HttpGet("index")]
        public IActionResult Index()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
            if (System.IO.File.Exists(filePath))
            {
                return PhysicalFile(filePath, "text/html");
            }
            else
            {
                return NotFound();
            }
        }
    }
}