using ApplicationLayer.Factories;
using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilitiesLayer.Helpers;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class MainController : Controller
    {
        private readonly SearchService _searchService;
        private readonly ViewModelFactory _viewModelFactory;

        public MainController(SearchService searchService, ViewModelFactory viewModelFactory)
        {
            _searchService = searchService;
            _viewModelFactory = viewModelFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated) 
            {
                Listener listener = await _searchService.FindUserByIdAsync<Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
                MainViewModel modelVM = await _viewModelFactory.FacMainVMAsync(await _viewModelFactory.FacMusicsViewModelByUserIdAsync<Listener>(listener.Id), listener.Id);
                return View(modelVM);
            }
            return View();
        }
               
        [HttpGet]
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }
    }
}
