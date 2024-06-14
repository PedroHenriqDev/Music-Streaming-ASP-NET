using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using PresentationLayer.SharedComponents.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtilitiesLayer.Helpers;
using System.Security.Claims;
using ApplicationLayer.Factories;
using ApplicationLayer.Services;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class ArtistController : UserController<Artist>
    {
        private readonly ViewModelFactory _viewModelFactory;
        private readonly RecordService _recordService;

        public ArtistController(LoginService<Artist> loginService,
                              SearchService searchService,
                              UserAuthenticationService authenticationService,
                              VerifyService verifyService,
                              PictureService pictureService,
                              UpdateService updateService,
                              ModelFactory modelFactory,
                              IHttpContextAccessor httpAccessor,
                              ViewModelFactory viewModelFactory,
                              RecordService recordService)
            : base(loginService, searchService, authenticationService, verifyService, pictureService, updateService, modelFactory, httpAccessor)
        {
            _viewModelFactory = viewModelFactory;
            _recordService = recordService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ArtistPage()
        {   
            try
            {
                ArtistPageViewModel artistPageVM = await _viewModelFactory.FacArtistPageVMAsync(await _searchService.FindUserByIdAsync<Artist>(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
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
            if (!_verifyService.VerifyUserGenres(artistVM))
            {
                TempData["InvalidGenres"] = "You must select at least one genre!";
                artistVM.Genres = HttpHelper.GetSessionValue<List<Genre>>(_httpAccessor, SessionKeys.UserSessionKey);
                return View("SelectGenres", artistVM);
            }
            try
            {
                if (_verifyService.VerifyUser(artistVM))
                {
                    HttpHelper.RemoveSessionValue(_httpAccessor, SessionKeys.UserSessionKey);
                    EntityQuery<Artist> entityQuery = await _recordService.CreateUserAsync(
                                                      new Artist(Guid.NewGuid().ToString(), artistVM.Name, EncryptHelper.EncryptPasswordSHA512(artistVM.Password), artistVM.Email, artistVM.PhoneNumber, artistVM.BirthDate, DateTime.Now));
                    if (entityQuery.Result) 
                    {
                        await _recordService.CreateUserGenresAsync(_modelFactory.FacUserGenres<Artist>(entityQuery.Entity.Id, artistVM.SelectedGenreIds));
                        await _authenticationService.SignInUserAsync(entityQuery.Entity);
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
            var descriptionVM = await _viewModelFactory.FacArtistDescriptionVMAsync(await _searchService.FindUserByIdAsync<Artist>(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return View(descriptionVM);
        }
    }
}
