using Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Diagnostics;
using System.Security.Claims;
using ViewModels;
using Models.ConcreteClasses;

namespace MusicWeaveListener.Controllers
{
    public class ListenerController : Controller
    {

        private readonly RecordUserService _recordUserService;
        private readonly LoginService _loginService;
        private readonly SearchService _searchService;
        private readonly PictureService _pictureService;
        private string _userEmail => User.FindFirst(ClaimTypes.Email)?.Value;
        private Listener _currentUser => _searchService.FindUserByEmail<Listener>(_userEmail);

        public ListenerController(
            RecordUserService recordUserService,
            LoginService loginService,
            SearchService searchService,
            PictureService pictureService) 
        {
            _recordUserService = recordUserService;
            _loginService = loginService;
            _searchService = searchService;
            _pictureService = pictureService;
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
        public async Task<IActionResult> RegisterListener(RegisterListenerViewModel listenerVM)
        {
            try
            {
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    await _recordUserService.CreateListenerAsync(listenerVM);
                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction(nameof(Login));
                }
                return View(listenerVM);
            }
            catch (RecordException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(listenerVM);
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

                if (ModelState.IsValid && await _loginService.LoginAsync<Listener>(credentialsVM))
                {
                    Listener user = await _searchService.FindEntityByEmailAsync<Listener>(credentialsVM.Email);
                    var claims = new List<Claim>();

                    if (user.PictureProfile != null)
                    {
                        string pictureUrl = await _pictureService.SavePictureProfileAsync(user.PictureProfile, HttpContext.Request.PathBase);
                        claims = new List<Claim>()
                        {
                            new Claim("PictureProfile", pictureUrl),
                        };
                    }
                    claims = new List<Claim>()
                    {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
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

                await _pictureService.AddPictureProfileAsync<Listener>(imageBase64, _currentUser);
                return RedirectToAction(nameof(UserPage));
            }
            catch (Exception ex)
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
