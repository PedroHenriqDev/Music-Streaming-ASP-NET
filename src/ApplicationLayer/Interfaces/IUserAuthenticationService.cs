using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface IUserAuthenticationService 
{
    Task SignInUserAsync<T>(T user)
     where T : IUser<T>;

    void SetCookie<T>(string key, T value);

    Task SignOutUserAsync();
}
