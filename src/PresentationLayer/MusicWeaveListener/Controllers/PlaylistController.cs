using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> AddPlaylistFoundMusics(string foundMusicsIds) 
        {
            var model = await _factoriesFacade.FacSearchMusicVMAsync(foundMusicsIds.ConvertStringJoinInList(), await _servicesFacade.FindCurrentListenerAsync());
            return View("AddPlaylistMusics", model);
        }
    }
}
