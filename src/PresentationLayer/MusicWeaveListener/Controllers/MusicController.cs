using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UtilitiesLayer.Helpers;

namespace MusicWeaveListener.Controllers
{
    public class MusicController : Controller
    {

        private readonly MusicServicesFacade<Listener> _servicesFacade;
        private readonly MusicFactoriesFacade _factoriesFacade;
        private static IDictionary<string, DateTime> _lastViewTime = new Dictionary<string, DateTime>();

        public MusicController(
            MusicServicesFacade<Listener> servicesFacade,
            MusicFactoriesFacade factoriesFacade)
        {
            _servicesFacade = servicesFacade;
            _factoriesFacade = factoriesFacade;
        }

        public async Task<IActionResult> RecordView([FromBody] string musicId)
        {
            
            var listener = await _servicesFacade.FindCurrentUserAsync();
            if (musicId is null || listener is null) 
            {
                return RedirectToAction(nameof(Error), new 
                {
                    message = "Any reference 'id' is null" 
                });
            }

            if(_lastViewTime.ContainsKey(listener.Id))
            {
                DateTime lastViewTime = _lastViewTime[listener.Id];
                if(!TimeHelper.HasElapsedSinceLastView(lastViewTime))
                {
                    return RedirectToAction("Index", "Main");
                }
            }

            _lastViewTime.Add(listener.Id, DateTime.Now);
            await _servicesFacade.CreateMusicViewAsync(_factoriesFacade.FacMusicView(Guid.NewGuid().ToString(), listener.Id, musicId, DateTime.Now));
            return RedirectToAction("Index", "Main");
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
