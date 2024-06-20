using ApplicationLayer.Interfaces;
using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilitiesLayer.Helpers;

namespace PresentationLayer.MusicWeaveArtist.Controllers;

public class MusicController : Controller
{
    private readonly IRecordService _recordService;
    private readonly ISearchService _searchService;
    private readonly IVerifyService _verifyService;
    private readonly IDeleteService _deleteService;

    public MusicController(IRecordService recordService,
                           ISearchService searchService,
                           IVerifyService verifyService, 
                           IDeleteService deleteService)
    { 
        _recordService = recordService;
        _searchService = searchService;
        _verifyService = verifyService;
        _deleteService = deleteService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();
        ViewBag.Genres = genres;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> CreateMusic(AddMusicViewModel musicVM, IFormFile musicImage, IFormFile musicAudio) 
    {
        TempData["AddMusicViewModel"] = JsonSerializationHelper.SerializeObject(musicVM);
        try
        {
            musicVM.PictureFile = musicImage;
            musicVM.AudioFile = musicAudio;
            await _recordService.RecordMusicAsync(musicVM, await _searchService.FindUserByIdAsync<Artist>(User.FindFirstValue(CookieKeys.UserIdCookieKey)));
            return RedirectToAction("ArtistPage", "Artist");
        }
        catch(MusicException ex) 
        {
            TempData["InvalidMusic"] = ex.Message;
            return View("AddMusicDatas", musicVM);
        }
        catch(ArgumentNullException ex)
        {
            TempData["InvalidMusic"] = ex.Message;
            return View("AddMusic", musicVM);
        }
        catch(Exception ex) 
        {
            return RedirectToAction("Error", new { message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> CreateMusicDatas(AddMusicViewModel musicVM) 
    {
        if (_verifyService.VerifyMusic(musicVM)) 
        {
            TempData["AddMusicViewModel"] = JsonSerializationHelper.SerializeObject(musicVM);
            return View(musicVM);
        }
        IEnumerable<Genre> genres = await _searchService.FindAllEntitiesAsync<Genre>();
        ViewBag.Genres = genres;
        return View("Index", musicVM);
    }
}
