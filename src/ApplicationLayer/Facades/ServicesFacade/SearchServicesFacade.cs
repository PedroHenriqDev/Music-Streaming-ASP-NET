using ApplicationLayer.Services;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.ServicesFacade
{
    public class SearchServicesFacade
    {
        private readonly SearchService _searchService;

        public SearchServicesFacade(SearchService searchService) 
        {
            _searchService = searchService;
        }

        public async Task<IEnumerable<Music>> FindMusicByQueryAsync(string query) 
        {
            return await _searchService.FindMusicsByQueryAsync(query);
        }
    }
}
