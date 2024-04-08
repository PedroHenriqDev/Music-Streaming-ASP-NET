using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Facades.ServicesFacade
{
    public class MusicServicesFacade
    {
        private readonly RecordService _recordService;
        private readonly SearchService _searchService;

        public MusicServicesFacade(RecordService recordService, SearchService searchService)
        {
            _recordService = recordService;
            _searchService = searchService;
        }

        public async Task<EntityQuery<Music>> CreateMusicAsync(AddMusicViewModel musicVM)
        {
            return await _recordService.CreateMusicAsync(musicVM);
        }

        public async Task<IEnumerable<T>> FindAllEntitiesAsync<T>() 
            where T : class, IEntity 
        {
            return await _searchService.FindAllEntitiesAsync<T>();
        }
    }
}
