using ApplicationLayer.ViewModels;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.HelpersFacade;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.SharedControllers;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class ArtistController : UserController<Artist>
    {
        private readonly UserServicesFacade<Artist> _servicesFacade;
        private readonly UserHelpersFacade<Artist> _helpersFacade;
        private readonly UserFactoriesFacade<Artist> _factoriesFacade;

        public ArtistController(
            UserServicesFacade<Artist> servicesFacade, 
            UserHelpersFacade<Artist> helpersFacade, 
            UserFactoriesFacade<Artist> factoriesFacade)
            : base(servicesFacade, helpersFacade, factoriesFacade)
        {
            _servicesFacade = servicesFacade;
            _helpersFacade = helpersFacade;
            _factoriesFacade = factoriesFacade;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterArtist()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ArtistPage()
        {
            try
            {
                ArtistPageViewModel artistPageVM = await _factoriesFacade.FacArtistPageVMAsync(await _servicesFacade.FindCurrentUserAsync());

                return View(artistPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RedirectToAddMusic()
        {
            return RedirectToAction("AddMusic", "Music");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterArtist(RegisterUserViewModel artistVM)
        {
            if (!artistVM.UserHaveGenres)
            {
                TempData["InvalidGenres"] = "You must select at least one genre!";
                artistVM.Genres = _helpersFacade.GetSessionValue<List<Genre>>("Genres");
                return View("SelectGenres", artistVM);
            }
            try
            {
                if (artistVM.UserIsValid)
                {
                    _helpersFacade.RemoveSessionValue("Genres");
                    EntityQuery<Artist> entityQuery = await _servicesFacade.CreateUserAsync(artistVM);
                    await _servicesFacade.SignInUserAsync(entityQuery.Entity);
                    return RedirectToAction(nameof(CompleteRegistration));
                }
                TempData["ErrorMessage"] = "Error creating object, some null parameter exists";
                return View(artistVM);
            }
            catch (RecordException<EntityQuery<Artist>> ex)
            {
                string message = $"Exception: {ex.Message}, result: {ex.EntityQuery.Result}, Query: {ex.EntityQuery.Message}, Moment: {ex.EntityQuery.Moment}";
                return RedirectToAction(nameof(Error), new { message = message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Description()
        {
            var descriptionVM = await _factoriesFacade.FacArtistDescriptionVMAsync(await _servicesFacade.FindCurrentUserAsync());
            return View(descriptionVM);
        }
    }
}
