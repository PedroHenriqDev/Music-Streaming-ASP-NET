using Datas.Sql;
using Exceptions;
using Models.ConcreteClasses;
using Models.Interfaces;

namespace Services
{
    public class SearchService
    {
        private readonly ConnectionDb _connectionDb;

        public SearchService(ConnectionDb connectionDb)
        {
            _connectionDb = connectionDb;
        }

        public T FindUserByEmail<T>(string email) where T : class, IEntityWithEmail<T>
        {
            return _connectionDb.GetUserByEmail<T>(email);
        }

        public async Task<T> FindEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            if (email == null) throw new SearchException("Email reference null");

            return await _connectionDb.GetEntityByEmailAsync<T>(email);
        }

        public async Task<T> FindUserByCredentialsAsync<T>(string email, string password)
            where T : IUser<T>
        {
            if (password == null || email == null) throw new SearchException("Password or email were used as a null reference");

            return await _connectionDb.GetEntityByCredentialsAsync<T>(email, password);
        }
    }
}
