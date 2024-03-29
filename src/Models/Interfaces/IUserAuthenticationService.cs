using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IUserAuthenticationService 
    {
        Task SignInUserAsync<T>(T user, HttpContext httpContext) where T : IUser<T>;
        Task SignOutUserAsync(HttpContext httpContext);
    }
}
