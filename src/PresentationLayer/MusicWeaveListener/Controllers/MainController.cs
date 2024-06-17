using ApplicationLayer.Mappings;
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
        private readonly ViewModelMapper _viewModelMapper;

        public MainController(SearchService searchService, ViewModelMapper viewModelMapper)
        {
            _searchService = searchService;
            _viewModelMapper = viewModelMapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View();

            Listener listener = await _searchService.FindUserByIdAsync<Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
            MainViewModel modelVM = await _viewModelMapper.ToMainViewModelAsync(await _viewModelMapper.ToMusicsViewModelByUserIdAsync<Listener>(listener.Id), listener.Id);
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
