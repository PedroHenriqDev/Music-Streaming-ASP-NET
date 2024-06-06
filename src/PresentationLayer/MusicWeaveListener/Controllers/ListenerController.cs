using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.SharedControllers;
using UtilitiesLayer.Helpers;
using System.Security.Claims;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class ListenerController : UserController<Listener>
    {
        private readonly UserServicesFacade<Listener> _servicesFacade;
        private readonly ListenerFactoriesFacade _factoriesFacade;
        private readonly UserFactoriesFacade<Listener> _userFactoriesFacade;
        private readonly IHttpContextAccessor _httpAccessor;

        public ListenerController(
            UserServicesFacade<Listener> servicesFacade,
            ListenerFactoriesFacade factoriesFacade,
            UserFactoriesFacade<Listener> userFactoriesFacade,
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
                if (!_servicesFacade.VerifyUserGenres(listenerVM))
                {
                    TempData["InvalidGenres"] = "You must select at least one genre!";
                    listenerVM.Genres = HttpHelper.GetSessionValue<List<Genre>>(_httpAccessor, SessionKeys.UserSessionKey);
                    return View("SelectGenres", listenerVM);
                }

                if (_servicesFacade.VerifyUser(listenerVM))
                {
                    HttpHelper.RemoveSessionValue(_httpAccessor, SessionKeys.UserSessionKey);
                    EntityQuery<Listener> listenerQuery = await _servicesFacade.CreateUserAsync(
                                                            new Listener(Guid.NewGuid().ToString(), listenerVM.Name, EncryptHelper.EncryptPasswordSHA512(listenerVM.Password), listenerVM.Email, listenerVM.PhoneNumber, listenerVM.BirthDate, DateTime.Now));
                    if (listenerQuery.Result) 
                    {
                        await _servicesFacade.CreateUserGenresAsync(_userFactoriesFacade.FacUserGenres(listenerQuery.Entity.Id, listenerVM.SelectedGenreIds));
                        await _servicesFacade.SignInUserAsync(listenerQuery.Entity);
                        return RedirectToAction(nameof(CompleteRegistration));
                    }
                    await _servicesFacade.DeleteEntityByIdAsync(listenerQuery.Entity.Id);
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
            var listenerPage = await _factoriesFacade.FacListenerPageVMAsync(await _servicesFacade.FindUserByIdAsync(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return View(listenerPage);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EditDescription()
        {
            var descriptionVM = await _factoriesFacade.FacListenerDescriptionVMAsync(await _servicesFacade.FindUserByIdAsync(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return View(descriptionVM);
        }
    }
}
