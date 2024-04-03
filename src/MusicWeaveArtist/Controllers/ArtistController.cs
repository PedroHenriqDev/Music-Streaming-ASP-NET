using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Exceptions;
using Datas;
using Services;
using System.Diagnostics;
using System.Security.Claims;
using System.Net;
using ViewModels;
using Newtonsoft.Json;
using Utilities.Helpers;
using System.Management;
using Facades;
using SharedControllers;
using Models.Entities;
using Models.Queries;

namespace MusicWeaveArtist.Controllers
{
    public class ArtistController : UserController<Artist>
    {
        private readonly UserServicesFacade<Artist> _servicesFacade;
        private readonly ILogger<ArtistController> _logger;

        public ArtistController(UserServicesFacade<Artist> servicesFacade, ILogger<ArtistController> logger) : base(servicesFacade)
        {
            _servicesFacade = servicesFacade;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterArtist()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ArtistPage()
        {
            try
            {
                ArtistPageViewModel artistPageVM = _servicesFacade.BuildArtistViewModel(await _servicesFacade.FindCurrentUserAsync());

                return View(artistPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RedirectToAddMusic()
        {
            return RedirectToAction("AddMusic", "Music");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterArtist(RegisterUserViewModel artistVM)
        {
            if (!artistVM.UserHaveGenres)
            {
                TempData["InvalidGenres"] = "You must select at least one genre!";
                artistVM.Genres = _servicesFacade.GetSessionValue<List<Genre>>("Genres");
                return View("SelectGenres", artistVM);
            }
            try
            {
                if (artistVM.UserIsValid)
                {
                    _servicesFacade.RemoveSessionValue("Genres");
                    EntityQuery<Artist> entityQuery = await _servicesFacade.CreateArtistAsync(artistVM);
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
    }
}
