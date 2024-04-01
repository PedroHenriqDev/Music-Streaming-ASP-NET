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

namespace MusicWeaveArtist.Controllers
{
    public class ArtistController : Controller
    {
        private readonly RecordUserService _recordUserService;
        private readonly LoginService _loginService;
        private readonly SearchService _searchService;
        private readonly UserAuthenticationService _authenticationService;
        private readonly PictureService _pictureService;
        private readonly JsonSerializationHelper _jsonHelper;
        private readonly VerifyService _verifyService;
        private readonly HttpHelper _httpHelper;
        private readonly UserPageService _userPageService;
        private readonly UpdateService _updateService;

        public ArtistController(
            RecordUserService recordUserService,
            LoginService loginService,
            SearchService searchService,
            PictureService pictureService,
            JsonSerializationHelper jsonHelper,
            UserAuthenticationService authenticationService,
            VerifyService verifyService,
            HttpHelper httpHelper,
            UserPageService userPageService,
            UpdateService updateService)
        {
            _recordUserService = recordUserService;
            _loginService = loginService;
            _searchService = searchService;
            _pictureService = pictureService;
            _jsonHelper = jsonHelper;
            _authenticationService = authenticationService;
            _verifyService = verifyService;
            _httpHelper = httpHelper;
            _userPageService = userPageService;
            _updateService = updateService;
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
        public async Task<IActionResult> Login()
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
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                if (ModelState.IsValid && await _loginService.LoginAsync<Artist>(credentialsVM))
                {
                    Artist artist = await _searchService.FindEntityByEmailAsync<Artist>(credentialsVM.Email);
                    await _authenticationService.SignInUserAsync(artist);
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            try
            {
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                await _authenticationService.SignOutUserAsync();
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
                if (Request.Method != "GET")
                {
                    return NotFound();
                }
                ArtistPageViewModel artistPageVM = _userPageService.BuildArtistViewModelAsync(await _searchService.FindCurrentUserAsync<Artist>());

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
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                Artist currentArtist = await _searchService.FindCurrentUserAsync<Artist>();
                await _pictureService.AddPictureProfileAsync(imageBase64, currentArtist);
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
                await _verifyService.VerifyDuplicateNameOrEmailAsync(artistVM.Name, artistVM.Email);
                if (artistVM.Step1IsValid)
                {
                    artistVM.Genres = (List<Genre>)await _searchService.FindAllEntitiesAsync<Genre>();
                    _httpHelper.SetSessionValue("Genres", artistVM.Genres);
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
                artistVM.Genres = _httpHelper.GetSessionValue<List<Genre>>("Genres");
                return View("SelectGenres", artistVM);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _httpHelper.RemoveSessionValue("Genres");
                    await _recordUserService.CreateArtistAsync(artistVM);

                    Artist currentArtist = await _searchService.FindEntityByEmailAsync<Artist>(artistVM.Email);
                    await _authenticationService.SignInUserAsync(currentArtist);

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
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

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

            return View(await _searchService.FindCurrentUserAsync<Artist>());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddDescription(Artist artist)
        {
            if (Request.Method != "POST")
            {
                return NotFound();
            }

            await _updateService.UpdateDescriptionAsync(artist);
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
