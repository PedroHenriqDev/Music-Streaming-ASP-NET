using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UtilitiesLayer.Extensions;
using UtilitiesLayer.Helpers;

namespace MusicWeaveListener.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly PlaylistServicesFacade _servicesFacade;
        private readonly PlaylistFactoriesFacades _factoriesFacade;
        private readonly IHttpContextAccessor _httpAccessor;
        private static readonly string _sessionPlaylists = EncryptHelper.GenerateEncryptedString();

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
                var listener = await _servicesFacade.FindCurrentListenerAsync();
                var playlistsVM = await _factoriesFacade.FacPlaylistViewModelsAsync(await _servicesFacade.FindPlaylistByListenerIdAsync(listener.Id));
                HttpHelper.SetSessionValue(_httpAccessor, _sessionPlaylists, playlistsVM);
                return View(playlistsVM);
            }
            catch(Exception ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Playlist(string playlistId) 
        {
            if(playlistId is null)
            {
                return RedirectToAction(nameof(Error), new 
                {
                    message = "An error ocurred, because: Playlist choice is null"
                });
            }

            var playlistsVM = HttpHelper.GetSessionValue<IEnumerable<PlaylistViewModel>>(_httpAccessor, _sessionPlaylists);
            var playlist = playlistsVM.FirstOrDefault(p => p.Id == playlistId);
            if(playlist is null) 
            {
                    return RedirectToAction(nameof(Error), new
                    {
                        message = "Playlist not found"
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

            var listener = await _servicesFacade.FindCurrentListenerAsync();
            var playlistVerify = _servicesFacade.VerifyPlaylistVM(playlistVM);
            try
            {
                if (playlistVerify.IsValid)
                {
                    EntityQuery<Playlist> playlistQuery = await _servicesFacade.RecordPlaylistAsnyc(await _factoriesFacade.FacPlaylistAsync(playlistVM, listener.Id));
                    if (playlistQuery.Result)
                    {
                        HttpHelper.SetSessionValue(_httpAccessor, "PlaylistId", playlistId);
                        return RedirectToAction(nameof(AddPlaylistMusics));
                    }
                    return View(playlistVM);
                }
                return View(playlistVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddPlaylistMusics()
        {
            var model = await _factoriesFacade.FacSearchMusicVMAsync(await _servicesFacade.FindCurrentListenerAsync());
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

            Listener listener = await _servicesFacade.FindCurrentListenerAsync();
            var playlistQuery = await _servicesFacade.CreatePlaylistMusicsAsync(_factoriesFacade.FacPlaylistMusics(HttpHelper.GetSessionValue<string>(_httpAccessor, "PlaylistId"), listener.Id, musicsToAdd.ConvertStringJoinInList()));
            HttpHelper.RemoveSessionValue(_httpAccessor, "PlaylistId");

            if (playlistQuery.Result)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Error), new { message = playlistQuery.Message });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddPlaylistFoundMusics(string foundMusicsIds)
        {
            var model = await _factoriesFacade.FacSearchMusicVMAsync(foundMusicsIds.ConvertStringJoinInList(), await _servicesFacade.FindCurrentListenerAsync());
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
