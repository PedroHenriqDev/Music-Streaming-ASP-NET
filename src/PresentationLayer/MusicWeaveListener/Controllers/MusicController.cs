using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
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
            var listenerId = User.FindFirstValue(CookieKeys.UserIdCookieKey);
            if (musicId is null || listenerId is null)
            {
                return RedirectToAction(nameof(Error), new
                {
                    message = "Any reference 'id' is null"
                });
            }

            if (_lastViewTime.ContainsKey(listenerId))
            {
                DateTime lastViewTime = _lastViewTime[listenerId];
                if (!TimeHelper.HasElapsedSinceLastView(lastViewTime))
                {
                    return RedirectToAction("Index", "Main");
                }
            }

            _lastViewTime.Add(listenerId, DateTime.Now);
            await _servicesFacade.CreateMusicViewAsync(_factoriesFacade.FacMusicView(Guid.NewGuid().ToString(), listenerId, musicId, DateTime.Now));
            return RedirectToAction("Index", "Main");
        }

        public async Task<IActionResult> AddFromFavorites([FromBody] string musicId)
        {
            if (musicId is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });
            }

            await _servicesFacade.CreateFavoriteMusicAsync(_factoriesFacade.FacFavoriteMusic(Guid.NewGuid().ToString(), musicId, User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return RedirectToAction("Index", "Main");
        }

        public async Task<IActionResult> MusicDetails(string musicId)
        {
            if (musicId is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });
            }

            var favoriteMusics = await _servicesFacade.GetEntitiesByFKAsync<FavoriteMusic, Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
            return View(await _factoriesFacade.FacMusicViewModelAsync(await _servicesFacade.FindDetailedMusicByIdAsync(musicId), favoriteMusics.Any(fm => fm.MusicId == musicId)));
        }

        public async Task<IActionResult> RemoveFromFavorites([FromBody] string musicId)
        {
            if (musicId is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });
            }

            await _servicesFacade.DeleteFavoriteMusicAsync(musicId, User.FindFirstValue(CookieKeys.UserIdCookieKey));
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
