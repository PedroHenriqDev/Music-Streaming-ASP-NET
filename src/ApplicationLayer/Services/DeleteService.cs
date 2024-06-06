using DataAccessLayer.Repositories;
using DataAccessLayer.Sql;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class DeleteService
    {

        private readonly VerifyService _verifyService;
        private readonly GenericRepository _genericRepository;
        private readonly MusicRepository _musicRepository;

        public DeleteService(VerifyService verifyService) 
        {
            _verifyService = verifyService;
        }

        public async Task DeleteEntityByIdAsync<T>(string id) 
            where T : class, IEntity
        {
            if (await _verifyService.HasEntityInDbAsync<T>(id)) 
            {
                await _genericRepository.RemoveEntityByIdAsync<T>(id);
            }
        }

        public async Task DeleteFavoriteMusic(string musicId, string listenerId)
        {
            await _musicRepository.RemoveFavoriteMusicAsync(musicId, listenerId);
        }
    }
}
