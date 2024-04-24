using ApplicationLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
