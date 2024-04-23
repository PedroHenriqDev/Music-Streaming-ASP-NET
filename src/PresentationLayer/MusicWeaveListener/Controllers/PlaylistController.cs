using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicWeaveListener.Controllers
{
    public class PlaylistController : Controller
    {

        private readonly PlaylistServicesFacade _servicesFacade;

        public PlaylistController(PlaylistServicesFacade servicesFacade)
        {
            _servicesFacade = servicesFacade;
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
                        return RedirectToAction(nameof(Index));
                    }
                    return View(playlistVM);
                }
                return View(playlistVM);
            }
            catch (Exception ex)
            {   
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }
    }
}
