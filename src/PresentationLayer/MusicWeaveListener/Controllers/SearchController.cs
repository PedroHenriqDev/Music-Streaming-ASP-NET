using Microsoft.AspNetCore.Mvc;

namespace MusicWeaveListener.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult SearchMusicToPlaylist(string query)
        {
            return View();
        }
    }
}
