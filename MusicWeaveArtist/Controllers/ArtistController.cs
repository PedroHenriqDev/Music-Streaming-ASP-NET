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

namespace MusicWeaveArtist.Controllers
{
    public class ArtistController : Controller
    {
        private readonly RecordUserService _recordUserService;
        private readonly LoginService _loginService;
        private readonly SearchService _searchService;
        private readonly PictureService _pictureService;
        private readonly JsonSerializationService _jsonService;
        private string _userEmail => User.FindFirst(ClaimTypes.Email)?.Value;
        private Artist _currentUser => _searchService.FindUserByEmail<Artist>(_userEmail);

        public ArtistController(
            RecordUserService recordUserService,
            LoginService loginService,
            SearchService searchService,
            PictureService pictureService,
            JsonSerializationService jsonService)
        {
            _recordUserService = recordUserService;
            _loginService = loginService;
            _searchService = searchService;
            _pictureService = pictureService;
            _jsonService = jsonService;
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
        public async Task<IActionResult> RegisterArtist(RegisterArtistViewModel artistVM)
        {
            try
            {
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    await _recordUserService.CreateArtistAsync(artistVM);
                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction(nameof(Login));
                }
                return View(artistVM);
            }
            catch (RecordException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(artistVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
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
                    var claims = new List<Claim>();

                    if (artist.PictureProfile != null)
                    {
                        string pictureUrl = await _pictureService.SavePictureProfileAsync(artist.PictureProfile, HttpContext.Request.PathBase);
                        claims = new List<Claim>()
                        {
                            new Claim("PictureProfile", pictureUrl),
                        };
                    }
                    claims = new List<Claim>()
                    {
                            new Claim(ClaimTypes.Name, artist.Name),
                            new Claim(ClaimTypes.Email, artist.Email),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
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

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(Login));
            }
            catch (BadHttpRequestException ex)
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

            return View(_currentUser);
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

                await _pictureService.AddPictureProfileAsync(imageBase64, _currentUser);
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
        public async Task<IActionResult> SelectGenres(RegisterArtistViewModel artistVM)
        {
            try
            {
                IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();

                string artistVMJson = _jsonService.SerializeObject(artistVM);

                TempData["RegisterArtistViewModel"] = artistVMJson;

                ViewBag.Genres = genres;
                return View(artistVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessRegistration(RegisterArtistViewModel artistVM, List<string> genreIds)
        {
            try
            {
                artistVM.GenreIds = genreIds;
                if (ModelState.IsValid)
                {
                    return View("RegisterArtist", artistVM);
                }
                return RedirectToAction(nameof(RegisterArtist));
            }
            catch(Exception ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
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
