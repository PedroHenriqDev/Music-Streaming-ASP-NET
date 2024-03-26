using MusicWeave.Datas;
using MusicWeave.Exceptions;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.ConcreteClasses;
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


        public User FindUserByEmail(string email)
        {
            return _connectionDb.GetUserByEmail(email);
        }

        public async Task<T> FindUserByEmailAsync<T>(string email) 
            where T : IEntityWithEmail<T>
        {
            if (email == null) throw new SearchException("Email reference null");

            return await _connectionDb.GetEntityByEmailAsync<T>(email);
        }

        public async Task<T> FindUserByCredentialsAsync<T>(string email, string password) 
            where T : IEntityWithEmail<T>
        {
            if (password == null || email == null) throw new SearchException("Password or email were used as a null reference");

            return await _connectionDb.GetEntityByCredentialsAsync<T>(email, password);
        }
    }
}
