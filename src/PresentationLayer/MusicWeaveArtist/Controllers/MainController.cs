using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class MainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
