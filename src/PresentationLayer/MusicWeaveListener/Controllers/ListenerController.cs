using ApplicationLayer.Facades.HelpersFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.SharedControllers;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class ListenerController : UserController<Listener>
    {
        private readonly UserServicesFacade<Listener> _servicesFacade;
        private readonly UserHelpersFacade<Listener> _helpersFacade;
        private readonly ListenerFactoriesFacade _listenerFactoriesFacade;
        private readonly UserFactoriesFacade<Listener> _userFactoriesFacade;

        public ListenerController(
            UserServicesFacade<Listener> servicesFacade,
            UserHelpersFacade<Listener> helpersFacade, 
            ListenerFactoriesFacade listenerFactoriesFacade,
            UserFactoriesFacade<Listener> userFactoriesFacade) 
            : base(servicesFacade, helpersFacade, userFactoriesFacade)
        {
            _servicesFacade = servicesFacade;
            _helpersFacade = helpersFacade;
            _listenerFactoriesFacade = listenerFactoriesFacade;
            _userFactoriesFacade = userFactoriesFacade;
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
                    listenerVM.Genres = _helpersFacade.GetSessionValue<List<Genre>>("Genres");
                    return View("SelectGenres", listenerVM);
                }

                if (_servicesFacade.VerifyUser(listenerVM))
                {
                    _helpersFacade.RemoveSessionValue("Genres");
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
            return View(_listenerFactoriesFacade.FacListenerPageVMAsync(await _servicesFacade.FindCurrentUserAsync()));
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
