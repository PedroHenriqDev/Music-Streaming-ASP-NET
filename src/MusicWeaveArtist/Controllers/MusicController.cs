using Microsoft.AspNetCore.Mvc;
using Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ViewModels;
using Models.ConcreteClasses;

namespace MusicWeaveArtist.Controllers
{
    public class MusicController : Controller
    {

        private readonly MusicService _musicService;
        private readonly SearchService _searchService;

        public MusicController(MusicService musicService, SearchService searchService) 
        {
            _musicService = musicService;
            _searchService = searchService;
        }

        public async Task<IActionResult> AddMusic()
        {
            IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View();
        }
    }
}
