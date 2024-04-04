using Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Diagnostics;
using System.Security.Claims;
using ViewModels;
using Models.Entities;
using SharedControllers;
using Models.Queries;
using Facades.Helpers;
using Facades.Factories;
using Facades.Services;

namespace MusicWeaveListener.Controllers
{
    public class ListenerController : UserController<Listener>
    {
        private readonly UserServicesFacade<Listener> _servicesFacade;
        private readonly UserHelpersFacade<Listener> _helpersFacade;
        private readonly UserFactoriesFacade<Listener> _factoriesFacade;


        public ListenerController(
            UserServicesFacade<Listener> servicesFacade,
            UserHelpersFacade<Listener> helpersFacade, 
            UserFactoriesFacade<Listener> factoriesFacade) 
            : base(servicesFacade, helpersFacade, factoriesFacade)
        {
            _servicesFacade = servicesFacade;
            _helpersFacade = helpersFacade;
            _factoriesFacade = factoriesFacade;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterListener()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterListener(RegisterUserViewModel listenerVM)
        {
            try
            {
                if (!listenerVM.UserHaveGenres)
                {
                    TempData["InvalidGenres"] = "You must select at least one genre!";
                    listenerVM.Genres = _helpersFacade.GetSessionValue<List<Genre>>("Genres");
                    return View("SelectGenres", listenerVM);
                }

                if (listenerVM.UserIsValid)
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
            if (Request.Method != "GET")
            {
                return NotFound();
            }

            return View(await _servicesFacade.FindCurrentUserAsync());
        }
    }
}
