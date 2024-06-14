using ApplicationLayer.Factories;
using ApplicationLayer.Services;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilitiesLayer.Helpers;

namespace MusicWeaveListener.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;
        private readonly ViewModelFactory _viewModelFactory;
        private readonly ILogger _logger;

        public SearchController(SearchService searchService, 
                                ViewModelFactory viewModelFactory, ILogger logger)
        {
            _searchService = searchService;
            _viewModelFactory = viewModelFactory;
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

            var playlistViewModel = await _viewModelFactory.FacSearchPlaylistViewModelAsync(await _searchService.FindPlaylistsByQueryAsync(query), User.FindFirstValue(CookieKeys.UserIdCookieKey));
            return View(playlistViewModel);
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

                var foundMusics = await _searchService.FindMusicsByQueryAsync(query);
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
