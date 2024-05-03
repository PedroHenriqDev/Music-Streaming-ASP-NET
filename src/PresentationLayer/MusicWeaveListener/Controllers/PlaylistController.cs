using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UtilitiesLayer.Extensions;

namespace MusicWeaveListener.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly PlaylistServicesFacade _servicesFacade;
        private readonly PlaylistFactoriesFacades _factoriesFacade;

        public PlaylistController(PlaylistServicesFacade servicesFacade, PlaylistFactoriesFacades factoriesFacades)
        {
            _servicesFacade = servicesFacade;
            _factoriesFacade = factoriesFacades;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreatePlaylist()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlaylist(PlaylistViewModel playlistVM, IFormFile playlistImage)
        {
            playlistVM.FileImage = playlistImage; 
            var playlistVerify = _servicesFacade.VerifyPlaylistVM(playlistVM);
            try
            {
                if (playlistVerify.IsValid)
                {
                    EntityQuery<Playlist> playlistQuery = await _servicesFacade.RecordPlaylistAsnyc(playlistVM);
                    if (playlistQuery.Result)
                    {
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
            var playlistQuery = await _servicesFacade.CreatePlaylistMusicsAsync(_factoriesFacade.FacPlaylistMusics(listener.Id, musicsToAdd.ConvertStringJoinInList()));
            
            if (playlistQuery.Result) 
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Error), new { message = playlistQuery.Message});
        }

        [HttpGet]
        public async Task<IActionResult> AddPlaylistFoundMusics(string foundMusicsIds) 
        {
            var model = await _factoriesFacade.FacSearchMusicVMAsync(foundMusicsIds.ConvertStringJoinInList(), await _servicesFacade.FindCurrentListenerAsync());
            return View("AddPlaylistMusics", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
