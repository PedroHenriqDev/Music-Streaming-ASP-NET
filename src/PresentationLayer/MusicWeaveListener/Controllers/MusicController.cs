using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MusicWeaveListener.Controllers
{
    public class MusicController : Controller
    {

        private readonly MusicServicesFacade<Listener> _servicesFacade;

        public MusicController(MusicServicesFacade<Listener> servicesFacade)
        {
            _servicesFacade = servicesFacade;
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
