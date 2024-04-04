using Datas.Sql;
using Exceptions;
using Models.Entities;
using Models.Interfaces;

namespace Services
{
    public class VerifyService
    {
        private readonly SearchService _searchService;

        public VerifyService(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<bool> HasNameInDbAsync<T>(string name)
            where T : class, IEntityWithName<T>
        {
            if (await _searchService.FindUserByNameAsync<T>(name) != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> HasEmailInDbAsync<T>(string email)
            where T : class, IEntityWithEmail<T>
        {
            if (await _searchService.FindEntityByEmailAsync<T>(email) != null)
            {
                return true;
            }
            return false;
        }

        public async Task VerifyDuplicateNameOrEmailAsync(string name, string email)
        {

            if (await HasNameInDbAsync<Listener>(name) || await HasNameInDbAsync<Artist>(name))
            {
                throw new EqualException("This name exist");
            }

            if (await HasEmailInDbAsync<Artist>(email) || await HasEmailInDbAsync<Listener>(email))
            {
                throw new EqualException("Existing email.");
            }
        }

        public async Task<bool> HasEntityInDbAsync<T>(string id)  where T : class, IEntity
        {
           return await _searchService.FindEntityByIdAsync<T>(id) != null;  
        }
    }
}
