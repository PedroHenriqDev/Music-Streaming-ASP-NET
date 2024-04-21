using Microsoft.AspNetCore.Mvc;

namespace MusicWeaveListener.Controllers
{
    public class PlaylistController : Controller
    {
        public IActionResult Playlists()
        {
            return View();
        }

        public IActionResult AddPlaylist() 
        {
            return View();
        }
    }
}
