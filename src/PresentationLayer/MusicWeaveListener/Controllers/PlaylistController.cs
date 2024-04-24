using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MusicWeaveListener.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly PlaylistServicesFacade _servicesFacade;
        private readonly ViewModelFactory _viewModelFactory;

        public PlaylistController(PlaylistServicesFacade servicesFacade, ViewModelFactory viewModelFactory)
        {
            _servicesFacade = servicesFacade;
            _viewModelFactory = viewModelFactory;
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
            return View(await _viewModelFactory.FacSearchMusicsVMAsync(await _servicesFacade.FindCurrentListenerAsync()));
        }
    }
}
