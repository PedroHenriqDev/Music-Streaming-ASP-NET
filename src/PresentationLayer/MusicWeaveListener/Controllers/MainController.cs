using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class MainController : Controller
    {
        private readonly MainFactoriesFacades _factoriesFacades;
        private readonly MainServicesFacade<Listener> _servicesFacade;

        public MainController(MainFactoriesFacades factoriesFacades, MainServicesFacade<Listener> servicesFacades)
        {
            _factoriesFacades = factoriesFacades;
            _servicesFacade = servicesFacades;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated) 
            {
                var completeMusicsVM = await _factoriesFacades.FacCompleteMusicsVMAsync(await _servicesFacade.FindCurrentUserAsync());
                return View(completeMusicsVM);
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
