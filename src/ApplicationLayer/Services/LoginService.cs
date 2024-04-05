using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class LoginService
    {
        private readonly EncryptService _encryptService;
        private readonly SearchService _searchService;

        public LoginService(
            EncryptService encryptService,
            SearchService searchService)
        {
            _encryptService = encryptService;
            _searchService = searchService;
        }

        public async Task<bool> LoginAsync<T>(LoginViewModel credentialsVM) where T : IUser<T>
        {
            return await _searchService.FindUserByCredentialsAsync<T>(credentialsVM.Email, _encryptService.EncryptPasswordSHA512(credentialsVM.Password)) != null;
        }
    }
}
