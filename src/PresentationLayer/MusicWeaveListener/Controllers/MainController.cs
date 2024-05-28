using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.MusicWeaveListener.Controllers
{
    public class MainController : Controller
    {
        private readonly MainFactoriesFacades _factoriesFacades;
        private readonly MainServicesFacade<Listener> _servicesFacade;

        public MainController(
            MainFactoriesFacades factoriesFacades, 
            MainServicesFacade<Listener> servicesFacades)
        {
            _factoriesFacades = factoriesFacades;
            _servicesFacade = servicesFacades;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated) 
            {
                var listener = await _servicesFacade.FindCurrentUserAsync();
                var modelVM = await _factoriesFacades.FacMainVMAsync(await _factoriesFacades.FacCompleteMusicsVMAsync(listener), listener.Id);
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
