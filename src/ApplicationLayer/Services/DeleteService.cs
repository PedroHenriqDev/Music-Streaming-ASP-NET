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
            if (await _verifyService.HasEntityInDbAsync<T>(id)) 
            {
                await _unitOfWork.GenericRepository.RemoveEntityByIdAsync<T>(id);
            }
        }

        public async Task DeleteFavoriteMusicAsync(string musicId, string listenerId)
        {
            await  _unitOfWork.MusicRepository.RemoveFavoriteMusicAsync(musicId, listenerId);
        }
    }
}
