using DataAccessLayer.Repositories;
using DataAccessLayer.Sql;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class UpdateService
    {

        private readonly UserRepository _userRepository;
        private readonly GenericRepository _genericRepository;

        public UpdateService(UserRepository userRepository, GenericRepository genericRepository)
        {
            _userRepository = userRepository;
            _genericRepository = genericRepository;
        }

        public async Task UpdateDescriptionAsync<T>(T entity) 
            where T : class, IEntityWithDescription<T>
        {
            await _genericRepository.UpdateDescriptionAsync(entity);
        }

        public async Task UpdateUserPictureProfileAsync<T>(T user)  
            where T : class, IUser<T>
        {
            await _userRepository.UpdateUserProfilePictureAsync(user);
        }
    }
}
