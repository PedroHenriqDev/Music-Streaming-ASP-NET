using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Logging;
using Models.ConcreteClasses;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {

        private readonly ILogger<UserAuthenticationService> _logger;
        private readonly PictureService _pictureService;

        public UserAuthenticationService(
            ILogger<UserAuthenticationService> logger,
            PictureService pictureService)
        {
            _logger = logger;
            _pictureService = pictureService;
        }

        public async Task SignInUserAsync<T>(T user, HttpContext httpContext) 
            where T : IUser<T>
        {
            if (user == null)
            {
                _logger.LogError("Error in authentication");
                throw new ArgumentNullException("Reference user null!");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };

            if (user.PictureProfile != null)
            {
                string pictureUrl = await _pictureService.SavePictureProfileAsync(user.PictureProfile, httpContext.Request.PathBase);
                claims.Add(new Claim("ProfilePictureUrl", pictureUrl));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties();

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public Task SignOutAsync(HttpContext context, string scheme, Microsoft.AspNetCore.Authentication.AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }

        public async Task SignOutUserAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
