using ApplicationLayer.Facades.ServicesFacade;
using Microsoft.AspNetCore.Mvc;

namespace MusicWeaveListener.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchServicesFacade _servicesFacades;

        public SearchController(SearchServicesFacade servicesFacades) 
        {
            _servicesFacades = servicesFacades;
        }

        public IActionResult SearchMusicToPlaylist(string query)
        {
            if(query is null) 
            {
                return RedirectToAction("AddPlaylistMusics", "Playlist");
            }

            return RedirectToAction("AddPlaylistMusics", "Playlist", new { musics = _servicesFacades.FindMusicByQueryAsync(query) });
        }
    }
}
