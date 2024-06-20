using ApplicationLayer.Interfaces;
using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Services;

public class LoginService<T> : ILoginService<T> where T : class, IUser<T>
{
    private readonly ISearchService _searchService;

    public LoginService(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<bool> LoginAsync(LoginViewModel credentialsVM)
    {
        return await _searchService.FindUserByCredentialsAsync<T>(credentialsVM.Email, EncryptHelper.EncryptPasswordSHA512(credentialsVM.Password)) != null;
    }
}
