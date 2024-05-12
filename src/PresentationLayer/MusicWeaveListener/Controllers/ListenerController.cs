using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.SharedControllers;
using UtilitiesLayer.Helpers;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class ListenerController : UserController<Listener>
    {
        private readonly UserServicesFacade<Listener> _servicesFacade;
        private readonly ListenerFactoriesFacade _listenerFactoriesFacade;
        private readonly UserFactoriesFacade<Listener> _userFactoriesFacade;
        private readonly IHttpContextAccessor _httpAccessor;

        public ListenerController(
            UserServicesFacade<Listener> servicesFacade,
            ListenerFactoriesFacade listenerFactoriesFacade,
            UserFactoriesFacade<Listener> userFactoriesFacade, 
            IHttpContextAccessor httpAccessor) 
            : base(servicesFacade, userFactoriesFacade, httpAccessor)
        {
            _servicesFacade = servicesFacade;
            _listenerFactoriesFacade = listenerFactoriesFacade;
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
                    listenerVM.Genres = HttpHelper.GetSessionValue<List<Genre>>(_httpAccessor, _sessionId);
                    return View("SelectGenres", listenerVM);
                }

                if (_servicesFacade.VerifyUser(listenerVM))
                {
                    HttpHelper.RemoveSessionValue(_httpAccessor, _sessionId);
                    EntityQuery<Listener> listenerQuery = await _servicesFacade.CreateUserAsync(listenerVM);
                    await _servicesFacade.SignInUserAsync(listenerQuery.Entity);
                    return RedirectToAction(nameof(CompleteRegistration));
                }
                TempData["ErrorMessage"] = "Error creating object, some null parameter exists";
                return View(listenerVM);
            }
            catch (RecordException<EntityQuery<Listener>> ex)
            {
                string message = $"Exception: {ex.Message}, result: {ex.EntityQuery.Result}, Query: {ex.EntityQuery.Message}, Moment: {ex.EntityQuery.Moment}";
                return RedirectToAction(nameof(Error), new { message = message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListenerPage()
        {
            return View(_listenerFactoriesFacade.FacListenerPageVM(await _servicesFacade.FindCurrentUserAsync()));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EditDescription()
        {
            var descriptionVM = await _listenerFactoriesFacade.FacListenerDescriptionVMAsync(await _servicesFacade.FindCurrentUserAsync());
            return View(descriptionVM);
        }
    }
}
