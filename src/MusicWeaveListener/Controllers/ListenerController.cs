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
using Facades;
using SharedControllers;
using Models.Queries;

namespace MusicWeaveListener.Controllers
{
    public class ListenerController : UserController<Listener>
    {
        private readonly UserServicesFacade<Listener> _servicesFacade;

        public ListenerController(UserServicesFacade<Listener> servicesFacade) : base(servicesFacade)
        {
            _servicesFacade = servicesFacade;
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
                    listenerVM.Genres = _servicesFacade.GetSessionValue<List<Genre>>("Genres");
                    return View("SelectGenres", listenerVM);
                }

                if (listenerVM.UserIsValid)
                {
                    _servicesFacade.RemoveSessionValue("Genres");
                    EntityQuery<Listener> listenerQuery = await _servicesFacade.CreateListenerAsync(listenerVM);
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
