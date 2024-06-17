using ApplicationLayer.Factories;
using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UtilitiesLayer.Helpers;

namespace MusicWeaveListener.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;
        private readonly ViewModelFactory _viewModelFactory;
        private readonly ILogger<SearchController> _logger;
        private readonly IHttpContextAccessor _httpAccessor;

        public SearchController(SearchService searchService, 
                                ViewModelFactory viewModelFactory, 
                                ILogger<SearchController> logger,
                                IHttpContextAccessor httpAccessor)
        {
            _searchService = searchService;
            _viewModelFactory = viewModelFactory;
            _logger = logger;
            _httpAccessor = httpAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SearchPlaylists()
        {
            return View();
        }
         
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchPlaylistsByQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) 
                return RedirectToAction(nameof(SearchPlaylists));

            var listenerId = User.FindFirstValue(CookieKeys.UserIdCookieKey);

            if (string.IsNullOrWhiteSpace(listenerId))
                return RedirectToAction(nameof(Error), new {messahge = "Listener is null"});

            HttpHelper.SetSessionValue(_httpAccessor, SessionKeys.QuerySessionKey, query);

            var playlistViewModel = await _viewModelFactory.FacSearchPlaylistViewModelAsync(await _searchService.FindPlaylistsByQueryAsync(query, listenerId), User.FindFirstValue(CookieKeys.UserIdCookieKey));
            return View("SearchPlaylists", playlistViewModel);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
