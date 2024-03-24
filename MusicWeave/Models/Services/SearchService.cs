using MusicWeave.Datas;
using MusicWeave.Models.Interfaces;

namespace MusicWeave.Models.Services
{
    public class SearchService
    {
        private readonly ConnectionDb _connectionDb;

        public SearchService(ConnectionDb connectionDb)
        {
            _connectionDb = connectionDb;
        }

        public async Task<T> ReturnUserByEmailAsync<T>(string email) where T : class, IUser<T>
        {
            if (email == null) throw new ArgumentNullException("Email reference null");

            return await _connectionDb.GetUserByEmailAsync<T>(email);
        }

        public async Task<T> ReturnUserByCredentialsAsync<T>(string email, string password) where T : class, IUser<T>
        {
            if (password == null || email == null) throw new ArgumentNullException("Password or email were used as a null reference");

            return await _connectionDb.GetUserByCredentialsAsync<T>(email, password);
        }
    }
}
