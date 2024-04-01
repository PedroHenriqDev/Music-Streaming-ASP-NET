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

        public ArtistController(
            RecordUserService recordUserService,
            LoginService loginService,
            SearchService searchService,
            PictureService pictureService,
            JsonSerializationHelper jsonHelper,
            UserAuthenticationService authenticationService,
            VerifyService verifyService,
            HttpHelper httpHelper)
        {
            _recordUserService = recordUserService;
            _loginService = loginService;
            _searchService = searchService;
            _pictureService = pictureService;
            _jsonHelper = jsonHelper;
            _authenticationService = authenticationService;
            _verifyService = verifyService;
            _httpHelper = httpHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterArtist()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterArtist(string action)
        {
            RegisterUserViewModel artistVM = _jsonHelper.DeserializeObject<RegisterUserViewModel>((string)TempData["ArtistVM"]);
            try
            {
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    await _recordUserService.CreateArtistAsync(artistVM);
                    Artist currentArtist = await _searchService.FindEntityByEmailAsync<Artist>(artistVM.Email);
                    await _authenticationService.SignInUserAsync(currentArtist);
                    return RedirectToAction(action);
                }
                return View(artistVM);
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
            return View("AddMusic", "Music");
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
                    _httpHelper.SetSessionValue("CurrentUser", artist);
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

                await _authenticationService.SignOutUserAsync(HttpContext);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UserPage()
        {
            if (Request.Method != "GET")
            {
                return NotFound();
            }
            return View(await _searchService.FindCurrentUserAsync<Artist>());
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

                Artist currentArtist = _httpHelper.GetSessionValue<Artist>("CurrentUser");
                await _pictureService.AddPictureProfileAsync(imageBase64, currentArtist);
                return RedirectToAction(nameof(UserPage));
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
                    IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();
                    ViewBag.Genres = genres;

                    string artistVMJson = _jsonHelper.SerializeObject(artistVM);

                    TempData["RegisterArtistViewModel"] = artistVMJson;
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
        public async Task<IActionResult> ProcessRegistration(RegisterUserViewModel artistVM, List<string> genreIds)
        {
            try
            {
                artistVM.GenreIds = genreIds;
                if (artistVM.Step2IsValid)
                {
                    TempData["ArtistVM"] = _jsonHelper.SerializeObject(artistVM);
                    return RedirectToAction(nameof(CompleteRegistration));
                }
                return RedirectToAction(nameof(RegisterArtist));
            }
            catch(Exception ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CompleteRegistration() 
        {
            if(TempData["ArtistVM"] is string artistVMJson) 
            {
                RegisterUserViewModel artistVM = _jsonHelper.DeserializeObject<RegisterUserViewModel>(artistVMJson);
                TempData["ArtistVM"] = _jsonHelper.SerializeObject(artistVM);
                return View(artistVM);
            }
            return RedirectToAction(nameof(SelectGenres));
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
