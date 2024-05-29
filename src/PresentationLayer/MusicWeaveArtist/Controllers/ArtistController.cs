using ApplicationLayer.ViewModels;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.FactoriesFacade;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.SharedControllers;
using UtilitiesLayer.Helpers;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class ArtistController : UserController<Artist>
    {
        private readonly UserServicesFacade<Artist> _servicesFacade;
        private readonly ArtistFactoriesFacade _artistFactoriesFacade;
        private readonly UserFactoriesFacade<Artist> _userFactoriesFacade;
        private readonly IHttpContextAccessor _httpAccessor;

        public ArtistController(
            UserServicesFacade<Artist> servicesFacade, 
            ArtistFactoriesFacade artistFactoriesFacade,
            UserFactoriesFacade<Artist> userFactoriesFacade, 
            IHttpContextAccessor httpAccessor)
            : base(servicesFacade, userFactoriesFacade, httpAccessor)
        {
            _servicesFacade = servicesFacade;
            _artistFactoriesFacade = artistFactoriesFacade;
            _userFactoriesFacade = userFactoriesFacade;
            _httpAccessor = httpAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ArtistPage()
        {   
            try
            {
                ArtistPageViewModel artistPageVM = await _artistFactoriesFacade.FacArtistPageVMAsync(await _servicesFacade.FindCurrentUserAsync());
                return View(artistPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RedirectToCreateMusic()
        {
            return RedirectToAction("CreateMusic", "Music");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateArtist()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateArtist(RegisterUserViewModel artistVM)
        {
            if (!_servicesFacade.VerifyUserGenres(artistVM))
            {
                TempData["InvalidGenres"] = "You must select at least one genre!";
                artistVM.Genres = HttpHelper.GetSessionValue<List<Genre>>(_httpAccessor, CookiesAndSessionsKeys.UserSessionKey);
                return View("SelectGenres", artistVM);
            }
            try
            {
                if (_servicesFacade.VerifyUser(artistVM))
                {
                    HttpHelper.RemoveSessionValue(_httpAccessor, CookiesAndSessionsKeys.UserSessionKey);
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
        public async Task<IActionResult> EditDescription()
        {
            var descriptionVM = await _artistFactoriesFacade.FacArtistDescriptionVMAsync(await _servicesFacade.FindCurrentUserAsync());
            return View(descriptionVM);
        }
    }
}
