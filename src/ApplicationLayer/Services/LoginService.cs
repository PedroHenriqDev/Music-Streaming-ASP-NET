using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Services
{
    public class LoginService<T> where T : class, IUser<T>
    {
        private readonly SearchService _searchService;

        public LoginService(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<bool> LoginAsync(LoginViewModel credentialsVM)
        {
            return await _searchService.FindUserByCredentialsAsync<T>(credentialsVM.Email, EncryptHelper.EncryptPasswordSHA512(credentialsVM.Password)) != null;
        }
    }
}
