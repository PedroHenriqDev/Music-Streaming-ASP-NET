using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using PresentationLayer.SharedComponents.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtilitiesLayer.Helpers;
using System.Security.Claims;
using ApplicationLayer.Services;
using ApplicationLayer.Factories;

namespace PresentationLayer.MusicWeaveArtist.Controllers
{
    public class ArtistController : UserController<Artist>
    {
        private readonly ViewModelFactory _viewModelFactory;
        private readonly RecordService _recordService;
        private readonly GenerateIntelliTextService _generateIntelliTextService;

        public ArtistController(LoginService<Artist> loginService,
                              SearchService searchService,
                              UserAuthenticationService authenticationService,
                              VerifyService verifyService,
                              PictureService pictureService,
                              UpdateService updateService,
                              DomainFactory domainCreationService,
                              IHttpContextAccessor httpAccessor,
                              ViewModelFactory viewModelFactory,
                              RecordService recordService, 
                              GenerateIntelliTextService generateIntelliTextService)
            : base(loginService, searchService, authenticationService, verifyService, pictureService, updateService, domainCreationService, httpAccessor)
        {
            _viewModelFactory = viewModelFactory;
            _recordService = recordService;
            _generateIntelliTextService = generateIntelliTextService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ArtistPage()
        {   
            try
            {
                ArtistPageViewModel artistPageVM = await _viewModelFactory.CreateArtistPageViewModelAsync(await _searchService.FindUserByIdAsync<Artist>(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
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
                    EntityQuery<Artist> entityQuery = await _recordService.RecordUserAsync(
                                                      new Artist(Guid.NewGuid().ToString(), artistVM.Name, EncryptHelper.EncryptPasswordSHA512(artistVM.Password), artistVM.Email, artistVM.PhoneNumber, artistVM.BirthDate, DateTime.Now));
                    if (entityQuery.Result) 
                    {
                        await _recordService.RecordUserGenresAsync(_domainCreationService.CreateUserGenres<Artist>(entityQuery.Entity.Id, artistVM.SelectedGenreIds));
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
            var artist = await _searchService.FindUserByIdAsync<Artist>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
            var descriptionVM = new DescriptionViewModel(artist.Description, artist.Name, artist.Id, await _generateIntelliTextService.GenerateArtistDescriptionAsync(artist));
            return View(descriptionVM);
        }
    }
}
