using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWeave.Exceptions;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.Services;
using MusicWeave.Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using System.Net;

namespace MusicWeave.Controllers
{
    public class UserController : Controller
    {
        private readonly RegisterUserService _registerService;
        private readonly LoginService _loginService;
        private readonly SearchService _searchService;
        private readonly PictureService _pictureService;
        private string _userEmail => User.FindFirst(ClaimTypes.Email)?.Value;

        public UserController(
            RegisterUserService registerService,
            LoginService loginService,
            SearchService searchService,
            PictureService pictureService)
        {
            _registerService = registerService;
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
                    throw new BadHttpRequestException("An brutal error ocurred in request!");
                }

                if (ModelState.IsValid)
                {
                    await _registerService.CreateListenerAsync(listenerVM);
                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction("User", "Login");
                }
                return View(listenerVM);
            }
            catch (RegisterException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(listenerVM);
            }
            catch (ConnectionDbException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (EncryptException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
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
                    throw new BadHttpRequestException("An brutal error ocurred in request!");
                }

                if (ModelState.IsValid)
                {
                    await _registerService.CreateArtistAsync(artistVM);
                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction("User", "Login");
                }
                return View(artistVM);
            }
            catch (RegisterException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(artistVM);
            }
            catch (ConnectionDbException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (EncryptException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
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
                    throw new BadHttpRequestException("An brutal error ocurred in request!");
                }

                if (ModelState.IsValid && await _loginService.LoginAsync(credentialsVM))
                {
                    var claims = new List<Claim>();
                    User user = await _searchService.FindUserByEmailAsync<User>(credentialsVM.Email);

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
            catch (EncryptException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (SearchException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (ConnectionDbException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
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
                    throw new BadHttpRequestException("An brutal error ocurred in request");
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
            try
            {
                if (Request.Method != "GET")
                {
                    throw new BadHttpRequestException("An brutal error ocurred in request");
                }
                
                    return View(await _searchService.FindUserByEmailAsync<User>(_userEmail));
            }
            catch (BadHttpRequestException ex) 
            {
                return RedirectToAction(nameof(Error), new {message = ex.Message});
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
