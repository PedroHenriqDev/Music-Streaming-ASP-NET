using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Services
{
    public class LoginService
    {
        private readonly SearchService _searchService;

        public LoginService(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<bool> LoginAsync<T>(LoginViewModel credentialsVM) where T : IUser<T>
        {
            return await _searchService.FindUserByCredentialsAsync<T>(credentialsVM.Email, EncryptHelper.EncryptPasswordSHA512(credentialsVM.Password)) != null;
        }
    }
}
