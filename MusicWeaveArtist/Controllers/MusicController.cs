using Microsoft.AspNetCore.Mvc;
using Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ViewModels;

namespace MusicWeaveArtist.Controllers
{
    public class MusicController : Controller
    {

        private readonly MusicService _musicService;

        public MusicController(MusicService musicService) 
        {
            _musicService = musicService;
        }

        public IActionResult AddMusic()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMusic(MusicViewModel musicViewModel)
        {
            try
            {
                if (musicViewModel == null && musicViewModel.File == null && musicViewModel.File.Length <= 0)
                {
                    return RedirectToAction(nameof(Error), "Null file reference.");
                }
                await _musicService.AddMusicAsync(musicViewModel);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }
    }
}
