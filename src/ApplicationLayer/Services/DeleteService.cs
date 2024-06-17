using DataAccessLayer.UnitOfWork;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class DeleteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly VerifyService _verifyService;

        public DeleteService(IUnitOfWork unitOfWork, VerifyService verifyService)
        {
            _unitOfWork = unitOfWork;
            _verifyService = verifyService;
        }

        public async Task DeleteEntityByIdAsync<T>(string id)
            where T : class, IEntity
        {
            try
            {
                if (await _verifyService.HasEntityInDbAsync<T>(id))
                {
                    await _unitOfWork.GenericRepository.RemoveEntityByIdAsync<T>(id);
                }
            }
            catch(Exception ex) 
            {
                throw new InvalidOperationException($"Error in delete {nameof(T)}, because {ex.Message}");
            }
        }

        public async Task DeleteFavoriteMusicAsync(string musicId, string listenerId)
        {
            try
            {
                await _unitOfWork.MusicRepository.RemoveFavoriteMusicAsync(musicId, listenerId);
            }
            catch(Exception ex) 
            {
                throw new InvalidOperationException($"Error in delete favorite music, because {ex.Message}");
            }
        }

        public async Task DeleteFavoritePlaylistAsync(string playlistId, string listenerId) 
        {
            try
            {
                await _unitOfWork.PlaylistRepository.RemoveFavoritePlaylistAsync(playlistId, listenerId);
            }
            catch(Exception ex) 
            {
                throw new InvalidOperationException($"Error in delete favorite playlist, because {ex.Message}");
            }
        }
    }
}

