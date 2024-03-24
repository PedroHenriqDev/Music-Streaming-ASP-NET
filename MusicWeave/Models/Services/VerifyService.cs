using MusicWeave.Data;
using MusicWeave.Models.Interfaces;

namespace MusicWeave.Models.Services
{
    public class VerifyService
    {
        private readonly ConnectionDb _connectionDb;

        public VerifyService(ConnectionDb connectionDb)
        {
            _connectionDb = connectionDb;
        }

        public async Task<bool> HasNameInDbAsync<T>(T entity) 
            where T : class, IEntityWithName<T>
        {
            if (await _connectionDb.GetEntityByNameAsync<T>(entity) != null) 
            {
                return true;
            }
            return false;
        }
    }
}
