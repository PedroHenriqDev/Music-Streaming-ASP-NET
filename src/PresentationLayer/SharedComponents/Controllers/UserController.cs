using ApplicationLayer.Factories;
using ApplicationLayer.Interfaces;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UtilitiesLayer.Helpers;

namespace PresentationLayer.SharedComponents.Controllers;

public class UserController<T> : Controller where T : class, IUser<T>, new()
{
    protected readonly ILoginService<T> _loginService;
    protected readonly ISearchService _searchService;
    protected readonly IUserAuthenticationService _authenticationService;
    protected readonly IVerifyService _verifyService;
    protected readonly IPictureService _pictureService;
    protected readonly IUpdateService _updateService;
    protected readonly DomainFactory _domainCreationService;
    protected readonly IHttpContextAccessor _httpAccessor;
    private string UserPageName => typeof(T).Name + "Page";
    private string CreateUser => $"Create{typeof(T).Name}";

    public UserController(ILoginService<T> loginService,
                          ISearchService searchService,
                          IUserAuthenticationService authenticationService, 
                          IVerifyService verifyService, 
                          IPictureService pictureService,
                          IUpdateService updateService,
                          DomainFactory domainCreationService,
                          IHttpContextAccessor httpAccessor)
    {
        _loginService = loginService;
        _searchService = searchService;
        _authenticationService = authenticationService;
        _verifyService = verifyService;
        _pictureService = pictureService;
        _httpAccessor = httpAccessor;
        _updateService = updateService;
        _domainCreationService = domainCreationService;
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
            if (ModelState.IsValid && await _loginService.LoginAsync(credentialsVM))
            {
                T user = await _searchService.FindEntityByEmailAsync<T>(credentialsVM.Email);
                await _authenticationService.SignInUserAsync(user);
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
            await _verifyService.VerifyDuplicateNameOrEmailAsync(userVM.Name, userVM.Email);
            if (_verifyService.VerifyUser(userVM))
            {
                userVM.Genres = (List<Genre>)await _searchService.FindAllEntitiesAsync<Genre>();
                HttpHelper.SetSessionValue(_httpAccessor, SessionKeys.UserSessionKey, userVM.Genres);
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
            await _authenticationService.SignOutUserAsync();
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
            T user = await _searchService.FindCurrentUserAsync<T>();
            await _pictureService.AddPictureProfileAsync(imageBase64, user);
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
        await _updateService.UpdateDescriptionAsync(_domainCreationService.CreateUser<T>(descriptionVM.Id, descriptionVM.Description));
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