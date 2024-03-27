using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModels;
using System.Diagnostics;

namespace MusicWeaveArtist.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MusicService _musicService;

        public HomeController(ILogger<HomeController> logger, MusicService musicService)
        {
            _logger = logger;
            _musicService = musicService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
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
                return RedirectToAction(nameof(Error), new { message = ex.Message});
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
