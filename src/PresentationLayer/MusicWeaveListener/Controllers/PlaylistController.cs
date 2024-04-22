using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicWeaveListener.Controllers
{
    public class PlaylistController : Controller
    {

        private readonly PlaylistServicesFacade _playlistServicesFacade;

        public PlaylistController(PlaylistServicesFacade playlistServicesFacade)
        {
            _playlistServicesFacade = playlistServicesFacade;
        }

        [HttpGet]
        public IActionResult Playlists()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddPlaylist()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlaylist(PlaylistViewModel playlistVM, IFormFile playlistImage)
        {
            playlistVM.FileImage = playlistImage; 
            try
            {
                if (playlistVM.StepOneIsValid)
                {
                    EntityQuery<Playlist> playlistQuery = await _playlistServicesFacade.RecordPlaylistAsnyc(playlistVM);
                    if (playlistQuery.Result)
                    {
                        return RedirectToAction(nameof(Playlists));
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
