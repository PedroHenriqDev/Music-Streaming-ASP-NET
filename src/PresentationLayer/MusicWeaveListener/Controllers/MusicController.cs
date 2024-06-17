﻿using ApplicationLayer.Mappings;
using ApplicationLayer.Services;
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
        private readonly RecordService _recordService;
        private readonly DomainCreationService _domainCreationService;
        private readonly SearchService _searchService;
        private readonly ViewModelMapper _viewModelMapper;
        private readonly DeleteService _deleteService;
        private static IDictionary<string, DateTime> _lastViewTime = new Dictionary<string, DateTime>();

        public MusicController(RecordService recordService,
                               DomainCreationService domainCreationService,
                               SearchService searchService, 
                               ViewModelMapper viewModelMapper,
                               DeleteService deleteService)
        {
            _recordService = recordService;
            _domainCreationService = domainCreationService;
            _searchService = searchService;
            _viewModelMapper = viewModelMapper;
            _deleteService = deleteService;
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
            await _recordService.RecordMusicViewAsync(_domainCreationService.CreateMusicView(Guid.NewGuid().ToString(), listenerId, musicId, DateTime.Now));
            return RedirectToAction("Index", "Main");
        }

        public async Task<IActionResult> AddFromFavorites([FromBody] string musicId)
        {
            if (musicId is null)
                return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });

            await _recordService.RecordFavoriteMusicAsync(_domainCreationService.CreateFavoriteMusic(Guid.NewGuid().ToString(), musicId, User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return RedirectToAction("Index", "Main");
        }

        public async Task<IActionResult> MusicDetails(string musicId)
        {
            if (musicId is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });
            }

            var favoriteMusics = await _searchService.FindEntitiesByFKAsync<FavoriteMusic, Listener>(User.FindFirstValue(CookieKeys.UserIdCookieKey));
            return View(await _viewModelMapper.ToMusicViewModelAsync(await _searchService.FindDetailedMusicAsync(musicId), favoriteMusics.Any(fm => fm.MusicId == musicId)));
        }

        public async Task<IActionResult> RemoveFromFavorites([FromBody] string musicId)
        {
            if (musicId is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Any reference null ocurred" });
            }

            await _deleteService.DeleteFavoriteMusicAsync(musicId, User.FindFirstValue(CookieKeys.UserIdCookieKey));
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