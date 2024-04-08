using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using UtilitiesLayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.HelpersFacade;

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
        public async Task<IActionResult> AddMusic()
        {
            IEnumerable<Genre> genres = await _servicesFacade.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddMusic(AddMusicViewModel musicVM, IFormFile musicImage, IFormFile musicAudio) 
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
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddMusicDatas(AddMusicViewModel musicVM) 
        {
            if (musicVM.Step1IsValid) 
            {
                TempData["AddMusicViewModel"] = _helpersFacade.SerializeObject(musicVM);
                return View(musicVM);
            }
            IEnumerable<Genre> genres = await _servicesFacade.FindAllEntitiesAsync<Genre>();
            ViewBag.Genres = genres;
            return View("AddMusic", musicVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
