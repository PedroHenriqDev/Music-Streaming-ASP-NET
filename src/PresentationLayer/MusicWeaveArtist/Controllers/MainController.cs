using ApplicationLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PresentationLayer.MusicWeaveArtist.Controllers;

public class MainController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string message)
    {
        return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
