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
using Models.ConcreteClasses;
using Newtonsoft.Json;
using Utilities.Helpers;
using System.Management;
using Facades;

namespace MusicWeaveArtist.Controllers
{
    public class ArtistController : Controller
    {
        private readonly UserServicesFacade<Artist> _servicesFacade;

        public ArtistController(UserServicesFacade<Artist> servicesFacade)
        {
            _servicesFacade = servicesFacade;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterArtist()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RedirectToAddMusic()
        {
            return RedirectToAction("AddMusic", "Music");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel credentialsVM)
        {
            try
            {
                if (ModelState.IsValid && await _servicesFacade.LoginAsync(credentialsVM))
                {
                    Artist artist = await _servicesFacade.FindEntityByEmailAsync<Artist>(credentialsVM.Email);
                    await _servicesFacade.SignInUserAsync(artist);
                    return RedirectToAction("Index", "Home");
                }
                TempData["InvalidUser"] = "Email or password incorrect!";
                return View(credentialsVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutPost()
        {
            try
            {
                await _servicesFacade.SignOutUserAsync();
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
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
        public IActionResult AddPictureProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddPictureProfile(string imageBase64)
        {
            try
            {
                Artist currentArtist = await _servicesFacade.FindCurrentUserAsync();
                await _servicesFacade.AddPictureProfileAsync(imageBase64, currentArtist);
                return RedirectToAction(nameof(ArtistPage));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SelectGenres(RegisterUserViewModel artistVM)
        {
            try
            {
                await _servicesFacade.VerifyDuplicateNameOrEmailAsync(artistVM.Name, artistVM.Email);
                if (artistVM.Step1IsValid)
                {
                    artistVM.Genres = (List<Genre>)await _servicesFacade.FindAllEntitiesAsync<Genre>();
                    _servicesFacade.SetSessionValue("Genres", artistVM.Genres);
                    return View(artistVM);
                }
                return View("RegisterArtist", artistVM);
            }
            catch (EqualException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("RegisterArtist", artistVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]  
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterArtist(RegisterUserViewModel artistVM)
        {
            if (!artistVM.Step2IsValid)
            {
                TempData["InvalidGenres"] = "You must select at least one genre!";
                artistVM.Genres = _servicesFacade.GetSessionValue<List<Genre>>("Genres");
                return View("SelectGenres", artistVM);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _servicesFacade.RemoveSessionValue("Genres");
                    await _servicesFacade.CreateArtistAsync(artistVM);

                    Artist currentArtist = await _servicesFacade.FindEntityByEmailAsync<Artist>(artistVM.Email);
                    await _servicesFacade.SignInUserAsync(currentArtist);

                    return RedirectToAction(nameof(CompleteRegistration));
                }
                TempData["ErrorMessage"] = "Error creating object, some null parameter exists";
                return RedirectToAction(nameof(RegisterArtist));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CompleteRegistration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult CompleteRegistration(string action)
        {
            try
            {
                return RedirectToAction(action);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Description()
        {
            if (Request.Method != "GET")
            {
                return NotFound();
            }

            return View(await _servicesFacade.FindCurrentUserAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddDescription(Artist artist)
        {
            await _servicesFacade.UpdateDescriptionAsync(artist);
            return RedirectToAction(nameof(ArtistPage));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
