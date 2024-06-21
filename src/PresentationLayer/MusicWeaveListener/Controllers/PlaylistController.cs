using ApplicationLayer.Factories;
using ApplicationLayer.Interfaces;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UtilitiesLayer.Extensions;
using UtilitiesLayer.Helpers;

namespace MusicWeaveListener.Controllers;

public class PlaylistController : Controller
{
    private readonly ViewModelFactory _viewModelFactory;
    private readonly DomainFactory _domainFactory;
    private readonly ISearchService _searchService;
    private readonly IVerifyService _verifyService;
    private readonly IRecordService _recordService;
    private readonly IDeleteService _deleteService;
    private readonly IHttpContextAccessor _httpAccessor;

    public PlaylistController(ViewModelFactory viewModelFactory,
                               DomainFactory domainFactory,
                               ISearchService searchService,
                               IVerifyService verifyService,
                               IRecordService recordService,
                               IDeleteService deleteService,
                               IHttpContextAccessor httpAccessor)
    {
        _viewModelFactory = viewModelFactory;
        _domainFactory = domainFactory;
        _searchService = searchService;
        _verifyService = verifyService;
        _recordService = recordService;
        _httpAccessor = httpAccessor;
        _deleteService = deleteService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            var playlistsVM = await _viewModelFactory.CreatePlaylistsViewModelAsync(await _searchService.FindPlaylistsByListenerIdAsync(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            HttpHelper.SetSessionValue(_httpAccessor, SessionKeys.PlaylistSessionKey, playlistsVM);
            return View(playlistsVM);
        }
        catch (Exception ex)
        {
            return RedirectToAction(nameof(Error), new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Playlist(string playlistId)
    {
        if (playlistId is null)
            return RedirectToAction(nameof(Error), new
            {
                message = "An error ocurred, because reference null"
            });

        var playlistViewModel = await _viewModelFactory.CreatePlaylistViewModelAsync(await _searchService.FindPlaylistByIdAsync(playlistId));

        return View(playlistViewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult PlaylistFromSession(string playlistId)
    {
        if (playlistId is null)
            return RedirectToAction(nameof(Error), new
            {
                message = "An error ocurred, because: Playlist choice is null"
            });

        var playlistsViewModel = HttpHelper.GetSessionValue<IEnumerable<PlaylistViewModel>>(_httpAccessor, SessionKeys.PlaylistSessionKey);
        var playlistViewModel = playlistsViewModel.FirstOrDefault(p => p.Id == playlistId);

        if (playlistViewModel is null)
            return RedirectToAction(nameof(Error), new
            {
                message = "Not found playlist"
            });

        return View("Playlist", playlistViewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult CreatePlaylist()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePlaylist(PlaylistViewModel playlistVM, IFormFile playlistImage)
    {
        playlistVM.FileImage = playlistImage;
        string playlistId = Guid.NewGuid().ToString();
        playlistVM.Id = playlistId;

        var playlistVerify = _verifyService.VefifyPlaylistVM(playlistVM);
        try
        {
            if (playlistVerify.IsValid)
            {
                EntityQuery<Playlist> playlistQuery = await _recordService.RecordPlaylistAsync(await _domainFactory.CreatePlaylistAsync(playlistVM, User.FindFirstValue(CookieKeys.UserIdCookieKey)));
                if (playlistQuery.Result)
                {
                    HttpHelper.SetSessionValue(_httpAccessor, SessionKeys.PlaylistIdSessionKey, playlistId);
                    return RedirectToAction(nameof(AddPlaylistMusics));
                }
                return View(playlistVM);
            }

            return View(playlistVM);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> DeletePlaylistMusic(string playlistId, string musicId, string controller, string action)
    {
        if (playlistId is null || musicId is null || action is null || controller is null)
        {
            return RedirectToAction(nameof(Error), new
            {
                message = $"Any reference null ocurred in {nameof(DeletePlaylistMusic)}"
            });
        }

        var playlistMusicQuery = await _deleteService.DeletePlaylistMusicAsync(_domainFactory.CreatePlaylistMusic(playlistId, musicId, User.FindFirstValue(CookieKeys.UserIdCookieKey)));

        if (!playlistMusicQuery.Result)
        {
            return RedirectToAction(nameof(Error), new
            {
                message = playlistMusicQuery.Message
            });
        }

        return RedirectToAction(action, controller, new
        {
            playlistId = playlistId
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> AddPlaylistMusics()
    {
        var model = await _viewModelFactory.CreateSearchMusicViewModelAsync(User.FindFirstValue(CookieKeys.UserIdCookieKey));
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> AddPlaylistMusics(string musicsToAdd)
    {
        if (string.IsNullOrWhiteSpace(musicsToAdd))
            return RedirectToAction(nameof(AddPlaylistMusics));

        var playlistQuery = await _recordService.RecordPlaylistMusicsAsync(_domainFactory.CreatePlaylistMusics(HttpHelper.GetSessionValue<string>(_httpAccessor, SessionKeys.PlaylistIdSessionKey), User.FindFirstValue(CookieKeys.UserIdCookieKey), musicsToAdd.ConvertStringJoinInList()));
        HttpHelper.RemoveSessionValue(_httpAccessor, SessionKeys.PlaylistIdSessionKey);

        if (playlistQuery.Result)
            return RedirectToAction(nameof(Index));

        return RedirectToAction(nameof(Error), new
        {
            message = playlistQuery.Message
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> AddPlaylistFoundMusics(string foundMusicsIds)
    {
        var model = await _viewModelFactory.CreateSearchMusicViewModelAsync(foundMusicsIds.ConvertStringJoinInList(), User.FindFirstValue(CookieKeys.UserIdCookieKey));
        return View("AddPlaylistMusics", model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> AddToFavorites(string playlistId, string controller, string action)
    {
        var listenerId = User.FindFirstValue(CookieKeys.UserIdCookieKey);
        if (playlistId is null || listenerId is null)
            return RedirectToAction(nameof(Error), new { message = "Playlist or listener is null" });

        var favoritePlaylistQuery = await _recordService.RecordFavoritePlaylistAsync(_domainFactory.CreateFavoritePlaylist(Guid.NewGuid().ToString(), playlistId, listenerId));

        if (favoritePlaylistQuery.Result)
        {
            var query = HttpHelper.GetSessionValue<string>(_httpAccessor, SessionKeys.QuerySessionKey);
            return RedirectToAction(action, controller, new { query });
        }

        return RedirectToAction(nameof(Error), new { message = favoritePlaylistQuery.Message });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> RemoveToFavorites(string playlistId, string controller, string action)
    {
        var listenerId = User.FindFirstValue(CookieKeys.UserIdCookieKey);
        if (playlistId is null || listenerId is null)
            return RedirectToAction(nameof(Error), new { message = "Playlist or listener is null" });

        var favoritePlaylistQuery = await _deleteService.DeleteFavoritePlaylistAsync(_domainFactory.CreateFavoritePlaylist(playlistId, listenerId));

        if (favoritePlaylistQuery.Result)
            return RedirectToAction(action, controller, new
            {
                query = HttpHelper.GetSessionValue<string>(_httpAccessor, SessionKeys.QuerySessionKey)
            });

        return RedirectToAction(nameof(Error), new { message = favoritePlaylistQuery.Message });
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
