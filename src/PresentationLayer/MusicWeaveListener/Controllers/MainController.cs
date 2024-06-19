using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilitiesLayer.Helpers;
using ApplicationLayer.Factories;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class MainController : Controller
    {
        private readonly SearchService _searchService;
        private readonly ViewModelFactory _viewModelFactory;

        public MainController(SearchService searchService, ViewModelFactory viewModelMapper)
        {
            _searchService = searchService;
            _viewModelFactory = viewModelMapper;
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
}
