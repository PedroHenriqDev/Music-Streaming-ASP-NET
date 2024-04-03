using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models.Interfaces;
using System.Security.Claims;

namespace Services
{
    public class UserAuthenticationService
    {

        private readonly ILogger<UserAuthenticationService> _logger;
        private readonly PictureService _pictureService;
        private readonly IHttpContextAccessor _httpAcessor;
        private readonly JsonSerializationHelper _jsonHelper;
        public UserAuthenticationService(
            ILogger<UserAuthenticationService> logger,
            PictureService pictureService,
            IHttpContextAccessor httpAccessor,
            JsonSerializationHelper jsonHelper)
        {
            _logger = logger;
            _pictureService = pictureService; 
            _httpAcessor = httpAccessor;
            _jsonHelper = jsonHelper;
        }

        public async Task SignInUserAsync<T>(T user) 
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
                string pictureUrl = await _pictureService.SavePictureProfileAsync(user.PictureProfile, _httpAcessor.HttpContext.Request.PathBase);
                claims.Add(new Claim("ProfilePictureUrl", pictureUrl));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties();

            await _httpAcessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public void SetCookie<T>(string key, T value)
        {
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            var serializedValue = _jsonHelper.SerializeObject(value);
            _httpAcessor.HttpContext.Response.Cookies.Append(key, serializedValue, options);
        }

        public async Task SignOutUserAsync()
        {
            await _httpAcessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
