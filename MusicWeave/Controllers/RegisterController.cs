using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWeave.Exceptions;
using MusicWeave.Models.Services;
using MusicWeave.Models.ViewModels;
using System.Diagnostics;

namespace MusicWeave.Controllers
{
    public class RegisterController : Controller
    {

        private readonly RegisterUserService _registerService;

        public RegisterController(RegisterUserService registerService)
        {
            _registerService = registerService;
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
                    return RedirectToAction("Login", "Login");
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
                    return RedirectToAction("Login", "Login");
                }
                return View(artistVM);
            }
            catch(RegisterException ex) 
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
