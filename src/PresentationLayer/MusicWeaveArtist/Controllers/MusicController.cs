using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilitiesLayer.Helpers;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class MusicController : Controller
    {
        private readonly RecordService _recordService;
        private readonly SearchService _searchService;
        private readonly VerifyService _verifyService;
        private readonly DeleteService _deleteService;

        public MusicController(RecordService recordService,
                               SearchService searchService,
                               VerifyService verifyService, 
                               DeleteService deleteService)
        { 
            _recordService = recordService;
            _searchService = searchService;
            _verifyService = verifyService;
            _deleteService = deleteService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateMusic(AddMusicViewModel musicVM, IFormFile musicImage, IFormFile musicAudio) 
        {
            TempData["AddMusicViewModel"] = JsonSerializationHelper.SerializeObject(musicVM);
            try
            {
                musicVM.PictureFile = musicImage;
                musicVM.AudioFile = musicAudio;
                await _recordService.RecordMusicAsync(musicVM, await _searchService.FindUserByIdAsync<Artist>(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
                return RedirectToAction("ArtistPage", "Artist");
            }
            catch(MusicException ex) 
            {
                TempData["InvalidMusic"] = ex.Message;
                return View("AddMusicDatas", musicVM);
            }
            catch(ArgumentNullException ex)
            {
                TempData["InvalidMusic"] = ex.Message;
                return View("AddMusic", musicVM);
            }
            catch(Exception ex) 
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateMusicDatas(AddMusicViewModel musicVM) 
        {
            if (_verifyService.VerifyMusic(musicVM)) 
            {
                TempData["AddMusicViewModel"] = JsonSerializationHelper.SerializeObject(musicVM);
                return View(musicVM);
            }
            IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View("Index", musicVM);
        }
    }
}
