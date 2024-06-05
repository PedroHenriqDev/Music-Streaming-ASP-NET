using ApplicationLayer.Facades.ServicesFacade;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MusicWeaveListener.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchServicesFacade _servicesFacades;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            SearchServicesFacade servicesFacades,
            ILogger<SearchController> logger)
        {
            _servicesFacades = servicesFacades;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SearchPlaylists()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchPlaylists(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) 
            {
                return RedirectToAction(nameof(SearchPlaylists));
            }

            var foundPlaylists = await _servicesFacades.FindPlaylistsByQueryAsync(query);
            return View(foundPlaylists);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchMusicToPlaylist(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return RedirectToAction("AddPlaylistMusics", "Playlist");
                }

                var foundMusics = await _servicesFacades.FindMusicByQueryAsync(query);
                return RedirectToAction("AddPlaylistFoundMusics", "Playlist", new 
                {
                    foundMusicsIds = string.Join(",", foundMusics.Select(m => m.Id))
                });
            }
            catch(QueryException ex)
            {
                _logger.LogError("An error occurred while the query was running: 'GetMusicsByQueryAsync'");
                return RedirectToAction("Error", "Main", new 
                {
                    message = ex.Message
                });
            }
            catch(Exception ex)
            {
                _logger.LogError("An unexpected error ocurred");
                return RedirectToAction("Error", "Main", new 
                {
                    message = ex.Message
                });
            }
        }
    }
}
