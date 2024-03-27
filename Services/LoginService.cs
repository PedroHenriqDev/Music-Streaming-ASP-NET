using Models.Interfaces;
using ViewModels;
using Datas.Sql;

namespace Services
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

        public async Task<bool> LoginAsync<T>(LoginViewModel credentialsVM) where T : IUser<T>
        {
            return await _searchService.FindUserByCredentialsAsync<T>(credentialsVM.Email, _encryptService.EncryptPasswordSHA512(credentialsVM.Password)) != null;
        }
    }
}
