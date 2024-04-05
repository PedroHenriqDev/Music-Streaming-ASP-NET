using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using DomainLayer.Entities;
using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.HelpersFacade;
using ApplicationLayer.Facades.ServicesFacade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ApplicationLayer.ViewModels;

namespace PresentationLayer.SharedControllers
{
    public class UserController<T> : Controller where T : class, IUser<T>, new()
    {

        private readonly UserServicesFacade<T> _servicesFacade;
        private readonly UserHelpersFacade<T> _helpersFacade;
        private readonly UserFactoriesFacade<T> _factoriesFacade;
        private string UserPageName => typeof(T).Name + "Page";
        private string RegisterUser => $"Register{typeof(T).Name}";

        public UserController(
            UserServicesFacade<T> servicesFacade,
            UserHelpersFacade<T> helpersFacade,
            UserFactoriesFacade<T> factoriesFacade)
        {
            _servicesFacade = servicesFacade;
            _helpersFacade = helpersFacade;
            _factoriesFacade = factoriesFacade;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SelectGenres(RegisterUserViewModel userVM)
        {
            try
            {
                await _servicesFacade.VerifyDuplicateNameOrEmailAsync(userVM.Name, userVM.Email);
                if (userVM.UserIsValid)
                {
                    userVM.Genres = (List<Genre>)await _servicesFacade.FindAllEntitiesAsync<Genre>();
                    _helpersFacade.SetSessionValue("Genres", userVM.Genres);
                    return View(userVM);
                }
                return View(RegisterUser, userVM);
            }
            catch (EqualException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(RegisterUser, userVM);
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
            var descriptionVM = _factoriesFacade.FactoryDescriptionViewModel<T>(await _servicesFacade.FindCurrentUserAsync());
            return View(descriptionVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddDescription(DescriptionViewModel entity)
        {
            await _servicesFacade.UpdateDescriptionAsync(_factoriesFacade.FactoryUser(entity.Id, entity.Description));
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