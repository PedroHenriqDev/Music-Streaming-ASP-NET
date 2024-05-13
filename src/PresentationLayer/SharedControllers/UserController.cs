using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UtilitiesLayer.Helpers;

namespace PresentationLayer.SharedControllers
{
    public class UserController<T> : Controller where T : class, IUser<T>, new()
    {
        private readonly UserServicesFacade<T> _servicesFacade;
        private readonly UserFactoriesFacade<T> _factoriesFacade;
        private readonly IHttpContextAccessor _httpAccessor;
        private string UserPageName => typeof(T).Name + "Page";
        private string CreateUser => $"Create{typeof(T).Name}";
        public static string _sessionId = EncryptHelper.GenerateEncryptedString();

        public UserController(
            UserServicesFacade<T> servicesFacade,
            UserFactoriesFacade<T> factoriesFacade, 
            IHttpContextAccessor httpAccessor)
        {
            _servicesFacade = servicesFacade;
            _factoriesFacade = factoriesFacade;
            _httpAccessor = httpAccessor;
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
                    T user = await _servicesFacade.FindEntityByEmailAsync<T>(credentialsVM.Email);
                    await _servicesFacade.SignInUserAsync(user);
                    return RedirectToAction("Index", "Main");
                }
                TempData["InvalidUser"] = "Email or password incorrect!";
                return View(credentialsVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new
                {
                    message = ex.Message 
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SelectGenres(RegisterUserViewModel userVM)
        {
            try
            {
                await _servicesFacade.VerifyDuplicateNameOrEmailAsync(userVM.Name, userVM.Email);
                if (_servicesFacade.VerifyUser(userVM))
                {
                    userVM.Genres = (List<Genre>)await _servicesFacade.FindAllEntitiesAsync<Genre>();
                    HttpHelper.SetSessionValue(_httpAccessor, _sessionId, userVM.Genres);
                    return View(userVM);
                }
                return View(CreateUser, userVM);
            }
            catch (EqualException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(CreateUser, userVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new
                {
                    message = ex.Message
                });
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
                return RedirectToAction(nameof(Error), new
                {
                    message = ex.Message 
                });
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
                T user = await _servicesFacade.FindCurrentUserAsync();
                await _servicesFacade.AddPictureProfileAsync(imageBase64, user);
                return RedirectToAction(UserPageName);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new 
                {
                    message = ex.Message
                });
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
                return RedirectToAction(nameof(Error), new
                { 
                    message = ex.Message 
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> EditDescription(DescriptionViewModel descriptionVM)
        {
            await _servicesFacade.UpdateDescriptionAsync(_factoriesFacade.FacUser(descriptionVM.Id, descriptionVM.Description));
            return RedirectToAction(UserPageName);
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