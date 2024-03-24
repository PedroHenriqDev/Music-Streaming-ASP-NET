using MusicWeave.Datas;
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
            Artist artist = await _searchService.ReturnUserByCredentialsAsync<Artist>(credentialsVM.Email, _encryptService.EncryptPasswordSHA512(credentialsVM.Password));
            Listener listener = await _searchService.ReturnUserByCredentialsAsync<Listener>(credentialsVM.Email, _encryptService.EncryptPasswordSHA512(credentialsVM.Password));
            return artist != null || listener != null;
        }
    }
}
