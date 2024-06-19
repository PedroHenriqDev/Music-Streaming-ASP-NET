using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.SharedComponents.Controllers;
using UtilitiesLayer.Helpers;
using System.Security.Claims;
using ApplicationLayer.Services;
using ApplicationLayer.Factories;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class ListenerController : UserController<Listener>
    {
        private readonly RecordService _recordService;
        private readonly DeleteService _deleteService;
        private readonly ViewModelFactory _viewModelFactory;
        private readonly GenerateIntelliTextService _generateIntelliTextService;

        public ListenerController(LoginService<Listener> loginService,
                              SearchService searchService,
                              UserAuthenticationService authenticationService,
                              VerifyService verifyService,
                              PictureService pictureService,
                              UpdateService updateService,
                              DomainFactory domainCreationService,
                              IHttpContextAccessor httpAccessor,
                              RecordService recordService, 
                              DeleteService deleteService,
                              ViewModelFactory viewModelFactory,
                              GenerateIntelliTextService generateIntelliTextService)
            : base(loginService, searchService, authenticationService, verifyService, pictureService, updateService, domainCreationService, httpAccessor)
        {
            _recordService = recordService;
            _deleteService = deleteService;
            _viewModelFactory = viewModelFactory;
            _generateIntelliTextService = generateIntelliTextService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateListener()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateListener(RegisterUserViewModel listenerVM)
        {
            try
            {
                if (!_verifyService.VerifyUserGenres(listenerVM))
                {
                    TempData["InvalidGenres"] = "You must select at least one genre!";
                    listenerVM.Genres = HttpHelper.GetSessionValue<List<Genre>>(_httpAccessor, SessionKeys.UserSessionKey);
                    return View("SelectGenres", listenerVM);
                }

                if (_verifyService.VerifyUser(listenerVM))
                {
                    HttpHelper.RemoveSessionValue(_httpAccessor, SessionKeys.UserSessionKey);
                    EntityQuery<Listener> listenerQuery = await _recordService.RecordUserAsync(
                                                            new Listener(Guid.NewGuid().ToString(), listenerVM.Name, EncryptHelper.EncryptPasswordSHA512(listenerVM.Password), listenerVM.Email, listenerVM.PhoneNumber, listenerVM.BirthDate, DateTime.Now));
                    if (listenerQuery.Result) 
                    {
                        await _recordService.RecordUserGenresAsync(_domainCreationService.CreateUserGenres<Listener>(listenerQuery.Entity.Id, listenerVM.SelectedGenreIds));
                        await _authenticationService.SignInUserAsync(listenerQuery.Entity);
                        return RedirectToAction(nameof(CompleteRegistration));
                    }
                    await _deleteService.DeleteEntityByIdAsync<Listener>(listenerQuery.Entity.Id);
                }
                TempData["ErrorMessage"] = "Error creating object, some null parameter exists";
                return View(listenerVM);
            }
            catch (RecordException<EntityQuery<Listener>> ex)
            {
                string message = $"Exception: {ex.Message}, result: {ex.EntityQuery.Result}, Query: {ex.EntityQuery.Message}, Moment: {ex.EntityQuery.Moment}";
                return RedirectToAction(nameof(Error), new
                {
                    message = message
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListenerPage()
        {
            var listenerPage = await _viewModelFactory.CreateListenerPageViewModelAsync(await _searchService.FindUserByIdAsync<Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return View(listenerPage);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EditDescription()
        {
            var listener = await _searchService.FindUserByIdAsync<Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
            var descriptionVM = new DescriptionViewModel(listener.Description, listener.Name, listener.Id, await _generateIntelliTextService.GenerateListenerDescriptionAsync(listener));
            return View(descriptionVM);
        }
    }
}
