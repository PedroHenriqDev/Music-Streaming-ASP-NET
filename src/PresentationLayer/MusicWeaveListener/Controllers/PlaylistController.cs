using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UtilitiesLayer.Extensions;
using UtilitiesLayer.Helpers;

namespace MusicWeaveListener.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly PlaylistServicesFacade _servicesFacade;
        private readonly PlaylistFactoriesFacades _factoriesFacade;
        private readonly IHttpContextAccessor _httpAccessor;

        public PlaylistController(
        PlaylistServicesFacade servicesFacade,
        PlaylistFactoriesFacades factoriesFacades,
        IHttpContextAccessor httpAccessor)
        {
            _servicesFacade = servicesFacade;
            _factoriesFacade = factoriesFacades;
            _httpAccessor = httpAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {
                var playlistsVM = await _factoriesFacade.FacPlaylistViewModelsAsync(await _servicesFacade.FindPlaylistByListenerIdAsync(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
                HttpHelper.SetSessionValue(_httpAccessor, SessionKeys.PlaylistSessionKey, playlistsVM);
                return View(playlistsVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Playlist(string playlistId)
        {
            if (playlistId is null)
            {
                return RedirectToAction(nameof(Error), new
                {
                    message = "An error ocurred, because reference null"
                });
            }

            return View(await _factoriesFacade.FacPlaylistViewModelAsync(await _servicesFacade.FindPlaylistByIdAsync(playlistId)));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PlaylistFromSession(string playlistId)
        {
            if (playlistId is null)
            {
                return RedirectToAction(nameof(Error), new
                {
                    message = "An error ocurred, because: Playlist choice is null"
                });
            }

            var playlistsVM = HttpHelper.GetSessionValue<IEnumerable<PlaylistViewModel>>(_httpAccessor, SessionKeys.PlaylistSessionKey);
            var playlist = playlistsVM.FirstOrDefault(p => p.Id == playlistId);
            if (playlist is null)
            {
                return RedirectToAction(nameof(Error), new
                {
                    message = "Not found playlist"
                });
            }

            return View(playlist);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreatePlaylist()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePlaylist(PlaylistViewModel playlistVM, IFormFile playlistImage)
        {
            playlistVM.FileImage = playlistImage;
            string playlistId = Guid.NewGuid().ToString();
            playlistVM.Id = playlistId;

            var playlistVerify = _servicesFacade.VerifyPlaylistVM(playlistVM);
            try
            {
                if (playlistVerify.IsValid)
                {
                    EntityQuery<Playlist> playlistQuery = await _servicesFacade.RecordPlaylistAsnyc(await _factoriesFacade.FacPlaylistAsync(playlistVM, User.FindFirstValue(CookieKeys.UserIdCookieKey)));
                    if (playlistQuery.Result)
                    {
                        HttpHelper.SetSessionValue(_httpAccessor, SessionKeys.PlaylistIdSessionKey, playlistId);
                        return RedirectToAction(nameof(AddPlaylistMusics));
                    }
                    return View(playlistVM);
                }
                return View(playlistVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddPlaylistMusics()
        {
            var model = await _factoriesFacade.FacSearchMusicVMAsync(User.FindFirstValue(CookieKeys.UserIdCookieKey));
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddPlaylistMusics(string musicsToAdd)
        {
            if (string.IsNullOrWhiteSpace(musicsToAdd))
            {
                return RedirectToAction(nameof(AddPlaylistMusics));
            }

            var playlistQuery = await _servicesFacade.CreatePlaylistMusicsAsync(_factoriesFacade.FacPlaylistMusics(HttpHelper.GetSessionValue<string>(_httpAccessor, SessionKeys.PlaylistIdSessionKey), User.FindFirstValue(CookieKeys.UserIdCookieKey), musicsToAdd.ConvertStringJoinInList()));
            HttpHelper.RemoveSessionValue(_httpAccessor, SessionKeys.PlaylistIdSessionKey);

            if (playlistQuery.Result)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Error), new
            {
                message = playlistQuery.Message
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddPlaylistFoundMusics(string foundMusicsIds)
        {
            var model = await _factoriesFacade.FacSearchMusicVMAsync(foundMusicsIds.ConvertStringJoinInList(), User.FindFirstValue(CookieKeys.UserIdCookieKey));
            return View("AddPlaylistMusics", model);
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
