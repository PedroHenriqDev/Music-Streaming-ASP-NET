using Microsoft.AspNetCore.Mvc;
using Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ViewModels;
using Models.ConcreteClasses;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace MusicWeaveArtist.Controllers
{
    public class MusicController : Controller
    {

        private readonly MusicService _musicService;
        private readonly SearchService _searchService;
        private readonly JsonSerializationHelper _jsonHelper;

        public MusicController(MusicService musicService,
            SearchService searchService,
            JsonSerializationHelper jsonHelper) 
        {
            _musicService = musicService;
            _searchService = searchService;
            _jsonHelper = jsonHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddMusic()
        {
            IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddMusic(IFormFile musicImage, IFormFile musicAudio) 
        {
            AddMusicViewModel musicVM = _jsonHelper.DeserializeObject<AddMusicViewModel>((string)TempData["AddMusicViewModel"]);
            try
            {
                musicVM.Picture = musicImage;
                musicVM.Audio = musicAudio;
                await _musicService.AddMusicAsync(musicVM);
                return RedirectToAction("UserPage", "Artist");
            }
            catch(ArgumentNullException ex)
            {
                TempData["InvalidMusic"] = ex.Message;
                return View("AddMusicDatas", musicVM);
            }
            catch(Exception ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult AddMusicDatas(AddMusicViewModel musicVM) 
        {
            if (musicVM.Step1IsValid) 
            {
                TempData["AddMusicViewModel"] = _jsonHelper.SerializeObject(musicVM);
                return View(musicVM);
            }
            return View("AddMusic", musicVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
