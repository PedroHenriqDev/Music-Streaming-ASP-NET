using DataAccessLayer.Sql;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class UpdateService
    {
        private readonly ConnectionDb _connectionDb;

        public UpdateService(ConnectionDb connectionDb)
        {
            _connectionDb = connectionDb;
        }

        public async Task UpdateDescriptionAsync<T>(T entity) 
            where T : class, IEntityWithDescription<T>
        {
            await _connectionDb.UpdateDescriptionAsync<T>(entity);
        }

        public async Task UpdateUserPictureProfileAsync<T>(T user)  
            where T : IUser<T>
        {
            await _connectionDb.UpdateUserProfilePictureAsync(user);
        }
    }
}
