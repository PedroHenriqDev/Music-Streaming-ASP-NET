using ApplicationLayer.Facades.HelpersFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class MusicController : Controller
    {

        private readonly MusicServicesFacade _servicesFacade;
        private readonly MusicHelpersFacade _helpersFacade;

        public MusicController(
            MusicServicesFacade servicesFacade, MusicHelpersFacade helpersFacade)
        {
            _servicesFacade = servicesFacade;
            _helpersFacade = helpersFacade;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Genre> genres = await _servicesFacade.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateMusic(AddMusicViewModel musicVM, IFormFile musicImage, IFormFile musicAudio) 
        {
            TempData["AddMusicViewModel"] = _helpersFacade.SerializeObject(musicVM);
            try
            {
                musicVM.Picture = musicImage;
                musicVM.Audio = musicAudio;
                await _servicesFacade.CreateMusicAsync(musicVM);
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
            if (musicVM.Step1IsValid) 
            {
                TempData["AddMusicViewModel"] = _helpersFacade.SerializeObject(musicVM);
                return View(musicVM);
            }
            IEnumerable<Genre> genres = await _servicesFacade.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View("CreateMusic", musicVM);
        }
    }
}
