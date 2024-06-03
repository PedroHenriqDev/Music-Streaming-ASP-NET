using ApplicationLayer.Services;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Facades.ServicesFacade
{
    public class MainServicesFacade<T> where T : class, IUser<T>
    {
        private readonly SearchService _searchService;

        public MainServicesFacade(SearchService searchService) 
        {
            _searchService = searchService;
        }
        
        public async Task<T> FindUserByIdAsync(string userId)
        {
            return await _searchService.FindUserByIdAsync<T>(userId);
        }
    }
}
