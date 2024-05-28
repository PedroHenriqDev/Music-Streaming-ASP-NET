using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Facades.ServicesFacade
{
    public class MusicServicesFacade<T> where T : class, IUser<T>
    {
        private readonly RecordService _recordService;
        private readonly SearchService _searchService;
        private readonly VerifyService _verifyService;
        private readonly DeleteService _deleteService;

        public MusicServicesFacade(
            RecordService recordService,
            SearchService searchService, 
            VerifyService verifyService,
            DeleteService deleteService)
        {
            _recordService = recordService;
            _searchService = searchService;
            _verifyService = verifyService;
            _deleteService = deleteService;
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

        public bool VerifyMusic(AddMusicViewModel musicVM) 
        {
            return _verifyService.VerifyMusic(musicVM);
        }

        public async Task<T> FindCurrentUserAsync() 
        {
            return await _searchService.FindCurrentUserAsync<T>();
        }

        public async Task<EntityQuery<MusicView>> CreateMusicViewAsync(MusicView musicView) 
        {
            return await _recordService.CreateMusicViewAsync(musicView);    
        }

        public async Task<EntityQuery<FavoriteMusic>> CreateFavoriteMusicAsync(FavoriteMusic favoriteMusic) 
        {
            return await _recordService.CreateFavoriteMusicAsync(favoriteMusic);
        }

        public async Task DeleteFavoriteMusicAsync(string musicId, string listenerId) 
        {
            await _deleteService.DeleteFavoriteMusic(musicId, listenerId);
        }
    }
}
