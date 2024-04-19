using Microsoft.AspNetCore.Mvc;
using ApplicationLayer.ViewModels;
using System.Diagnostics;
using ApplicationLayer.Facades.FactoriesFacade;
using DomainLayer.Entities;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;
        private readonly HomeFactoriesFacades _factoriesFacades;

        public MainController(ILogger<MainController> logger, HomeFactoriesFacades factoriesFacades)
        {
            _logger = logger;
            _factoriesFacades = factoriesFacades;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated) 
            {
                var displayMusicVM = await _factoriesFacades.FacDisplayMusicVMAsync<Listener>();
                return View(displayMusicVM);
            }
            return View();
        }

        public IActionResult Playlist() 
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
