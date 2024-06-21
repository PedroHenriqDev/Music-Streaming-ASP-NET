using ApplicationLayer.Factories;
using ApplicationLayer.Interfaces;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UtilitiesLayer.Helpers;

namespace MusicWeaveListener.Controllers;

public class MusicController : Controller
{
    private readonly IRecordService _recordService;
    private readonly DomainFactory _domainFactory;
    private readonly ISearchService _searchService;
    private readonly ViewModelFactory _viewModelFactory;
    private readonly IDeleteService _deleteService;
    private static IDictionary<string, DateTime> _lastViewTime = new Dictionary<string, DateTime>();

    public MusicController(IRecordService recordService,
                           DomainFactory domainFactory,
                           ISearchService searchService, 
                           ViewModelFactory viewModelFactory,
                           IDeleteService deleteService)
    {
        _recordService = recordService;
        _domainFactory = domainFactory;
        _searchService = searchService;
        _viewModelFactory = viewModelFactory;
        _deleteService = deleteService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
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
        await _recordService.RecordMusicViewAsync(_domainFactory.CreateMusicView(Guid.NewGuid().ToString(), listenerId, musicId, DateTime.Now));
        return RedirectToAction("Index", "Main");
    }

    [HttpGet]
    public async Task<IActionResult> MusicDetails(string musicId)
    {
        if (musicId is null)
            return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });

        var favoriteMusics = await _searchService.FindEntitiesByFKAsync<FavoriteMusic, Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
        return View(await _viewModelFactory.CreateMusicViewModelAsync(await _searchService.FindDetailedMusicAsync(musicId), favoriteMusics.Any(fm => fm.MusicId == musicId)));
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToFavorites(string musicId, string controller, string action)
    {
        if (musicId is null)
            return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });

        await _recordService.RecordFavoriteMusicAsync(_domainFactory.CreateFavoriteMusic(Guid.NewGuid().ToString(), musicId, User.FindFirstValue(CookieKeys.UserIdCookieKey)));
        return RedirectToAction(action, controller);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveToFavorites(string musicId, string controller, string action)
    {
        if (musicId is null)
            return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });

        var favoriteMusicQuery = await _deleteService.DeleteFavoriteMusicAsync(_domainFactory.CreateFavoriteMusic(musicId, User.FindFirstValue(CookieKeys.UserIdCookieKey)));

        if (!favoriteMusicQuery.Result) 
            return RedirectToAction(nameof(Error), new { message = favoriteMusicQuery.Message});

        return RedirectToAction(action, controller);
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
