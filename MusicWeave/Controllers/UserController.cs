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

namespace MusicWeave.Controllers
{
    public class UserController : Controller
    {
        private readonly RegisterUserService _registerService;
        private readonly LoginService _loginService;
        private readonly SearchService _searchService;

        public UserController(
            RegisterUserService registerService,
            LoginService loginService, 
            SearchService searchService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _searchService = searchService;
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
                if (ModelState.IsValid && await _loginService.LoginAsync(credentialsVM))
                {
                    User user = await _searchService.FindUserByEmailAsync<User>(credentialsVM.Email);

                    var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email)
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
