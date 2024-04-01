using Datas.Sql;
using Models.Interfaces;
using Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
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
