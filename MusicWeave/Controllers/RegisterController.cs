using Microsoft.AspNetCore.Mvc;

namespace MusicWeave.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
