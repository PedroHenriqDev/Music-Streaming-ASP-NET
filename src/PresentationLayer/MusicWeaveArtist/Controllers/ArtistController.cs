using ApplicationLayer.ViewModels;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.FactoriesFacade;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.SharedControllers;
using UtilitiesLayer.Helpers;
using System.Security.Claims;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class ArtistController : UserController<Artist>
    {
        private readonly UserServicesFacade<Artist> _servicesFacade;
        private readonly ArtistFactoriesFacade _factoriesFacade;
        private readonly UserFactoriesFacade<Artist> _userFactoriesFacade;
        private readonly IHttpContextAccessor _httpAccessor;

        public ArtistController(
            UserServicesFacade<Artist> servicesFacade, 
            ArtistFactoriesFacade factoriesFacade,
            UserFactoriesFacade<Artist> userFactoriesFacade, 
            IHttpContextAccessor httpAccessor)
            : base(servicesFacade, userFactoriesFacade, httpAccessor)
        {
            _servicesFacade = servicesFacade;
            _factoriesFacade = factoriesFacade;
            _userFactoriesFacade = userFactoriesFacade;
            _httpAccessor = httpAccessor;
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
                artistVM.Genres = HttpHelper.GetSessionValue<List<Genre>>(_httpAccessor, SessionKeys.UserSessionKey);
                return View("SelectGenres", artistVM);
            }
            try
            {
                if (_servicesFacade.VerifyUser(artistVM))
                {
                    HttpHelper.RemoveSessionValue(_httpAccessor, SessionKeys.UserSessionKey);
                    EntityQuery<Artist> entityQuery = await _servicesFacade.CreateUserAsync(
                                                      new Artist(Guid.NewGuid().ToString(), artistVM.Name, EncryptHelper.EncryptPasswordSHA512(artistVM.Password), artistVM.Email, artistVM.PhoneNumber, artistVM.BirthDate, DateTime.Now));
                    if (entityQuery.Result) 
                    {
                        await _servicesFacade.CreateUserGenresAsync(_userFactoriesFacade.FacUserGenres(entityQuery.Entity.Id, artistVM.SelectedGenreIds));
                        await _servicesFacade.SignInUserAsync(entityQuery.Entity);
                        return RedirectToAction(nameof(CompleteRegistration));
                    }
                    
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
            var descriptionVM = await _factoriesFacade.FacArtistDescriptionVMAsync(await _servicesFacade.FindUserByIdAsync(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return View(descriptionVM);
        }
    }
}
