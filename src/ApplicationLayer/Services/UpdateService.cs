using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class UpdateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateDescriptionAsync<T>(T entity) 
            where T : class, IEntityWithDescription<T>
        {
            await _unitOfWork.GenericRepository.UpdateDescriptionAsync(entity);
        }

        public async Task UpdateUserPictureProfileAsync<T>(T user)  
            where T : class, IUser<T>
        {
            await _unitOfWork.UserRepository.UpdateUserProfilePictureAsync(user);
        }
    }
}
