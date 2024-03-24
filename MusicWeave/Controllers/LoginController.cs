using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWeave.Exceptions;
using MusicWeave.Models.Services;
using MusicWeave.Models.ViewModels;
using System.Security.Claims;

namespace MusicWeave.Controllers
{
    public class LoginController : Controller
    {

        private readonly LoginService _loginService;

        public LoginController(LoginService loginService) 
        {
            _loginService = loginService;
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
