using MusicWeave.Datas;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.Interfaces;
using MusicWeave.Models.ViewModels;

namespace MusicWeave.Models.Services
{
    public class LoginService
    {
        private readonly EncryptService _encryptService;
        private readonly ConnectionDb _connectionDb;
        private readonly SearchService _searchService;

        public LoginService(
            EncryptService encryptService,
            ConnectionDb connectionDb,
            SearchService searchService)
        {
            _encryptService = encryptService;
            _connectionDb = connectionDb;
            _searchService = searchService;
        }

        public async Task<bool> LoginAsync(LoginViewModel credentialsVM)
        {
            User user = await _searchService.FindUserByCredentialsAsync<User>(credentialsVM.Email, _encryptService.EncryptPasswordSHA512(credentialsVM.Password));
            return user != null;
        }
    }
}
