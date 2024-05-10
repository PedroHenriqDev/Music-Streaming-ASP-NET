using DataAccessLayer.Sql;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class DeleteService
    {

        private readonly ConnectionDb _connectionDb;
        private readonly VerifyService _verifyService;

        public DeleteService(
            ConnectionDb connectionDb,
            VerifyService verifyService) 
        {
            _connectionDb = connectionDb;
            _verifyService = verifyService;
        }

        public async Task DeleteEntityByIdAsync<T>(string id) 
            where T : class, IEntity
        {
            if (await _verifyService.HasEntityInDbAsync<T>(id)) 
            {
                await _connectionDb.DeleteEntityByIdAsync<T>(id);
            }
        }
    }
}
