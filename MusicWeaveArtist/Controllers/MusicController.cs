using Microsoft.AspNetCore.Mvc;

namespace MusicWeaveArtist.Controllers
{
    public class MusicController : Controller
    {
        public IActionResult AddMusic()
        {
            return View();
        }
    }
}
