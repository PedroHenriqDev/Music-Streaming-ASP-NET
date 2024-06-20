using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilitiesLayer.Helpers;
using ApplicationLayer.Factories;
using ApplicationLayer.Interfaces;

namespace PresentationLayer.MusicWeaveListener.Controllers;

public class MainController : Controller
{
    private readonly ISearchService _searchService;
    private readonly ViewModelFactory _viewModelFactory;

    public MainController(ISearchService searchService, 
                          ViewModelFactory viewModelFactory)
    {
        _searchService = searchService;
        _viewModelFactory = viewModelFactory;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
            return View();

        Listener listener = await _searchService.FindUserByIdAsync<Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
        MainViewModel modelVM = await _viewModelFactory.CreateMainViewModelAsync(await _viewModelFactory.CreateMusicsViewModelByUserIdAsync<Listener>(listener.Id), listener.Id);
        return View(modelVM);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult About()
    {
        return View();
    }
}
