using DataAccessLayer.UnitOfWork;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Services
{
    public class SearchService
    {
        private readonly IHttpContextAccessor _httpAcessor;
        private readonly IUnitOfWork _unitOfWork;

        public SearchService(IHttpContextAccessor httpAcessor,
                             IUnitOfWork unitOfWork)
        {
            _httpAcessor = httpAcessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<T> FindUserByNameAsync<T>(string name)
            where T : class, IUser<T>
        {
            return await _unitOfWork.UserRepository.GetUserByNameAsync<T>(name);
        }

        public async Task<T> FindUserByIdAsync<T>(string id)
            where T : class, IUser<T>
        {
            return await _unitOfWork.UserRepository.GetUserByIdAsync<T>(id);
        }

        public async Task<T> FindEntityByIdAsync<T>(string id)
            where T : class, IEntity
        {
            return await _unitOfWork.GenericRepository.GetEntityByIdAsync<T>(id);
        }

        public async Task<IEnumerable<T>> FindEntitiesByFKAsync<T, TR>(string fkId) 
            where T : class, IEntity where TR : class, IEntity 
        {
            return await _unitOfWork.GenericRepository.GetEntitiesByFKAsync<T, TR>(fkId);
        }

        public async Task<T> FindCurrentUserAsync<T>()
            where T : class, IUser<T>
        {
            return await _unitOfWork.UserRepository.GetUserByNameAsync<T>(_httpAcessor.HttpContext.User.Identity.Name);
        }

        public async Task<IEnumerable<Genre>> FindUserGenresAsync<T>(string userId)
            where T : class, IUser<T>
        {
            var userGenres = await _unitOfWork.EntitiesAssociationRepository.GetUserGenresAsync<T>(userId);
            return await _unitOfWork.GenericRepository.GetEntitiesByIdsAsync<Genre>(userGenres.Select(g => g.GenreId).ToList());
        }

        public async Task<IEnumerable<T>> FindAllEntitiesAsync<T>()
            where T : class, IEntity
        {
            return await _unitOfWork.GenericRepository.GetAllEntitiesAsync<T>();
        }

        public async Task<T> FindEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            if (email is null) throw new SearchException("Email reference null");

            return await _unitOfWork.GenericRepository.GetEntityByEmailAsync<T>(email);
        }

        public async Task<T> FindUserByCredentialsAsync<T>(string email, string password)
            where T : IUser<T>
        {
            if (password is null || email is null) throw new SearchException("Password or email were used as a null reference");

            return await _unitOfWork.UserRepository.GetUserByCredentialsAsync<T>(email, password);
        }

        public async Task<IEnumerable<Music>> FindMusicsByFkIdsAsync<T>(IEnumerable<string> fkIds) 
            where T : class, IEntity
        {
            return await _unitOfWork.MusicRepository.GetMusicsByFkIdsAsync<T>(fkIds);
        }

        public async Task<IEnumerable<Music>> FindMusicByFkIdAsync<T>(string fkId) 
            where T : class, IEntity
        {
            return await _unitOfWork.MusicRepository.GetMusicsByFkIdAsync<T>(fkId);
        }

        public async Task<IEnumerable<Music>> FindMusicsByQueryAsync(string query) 
        {
            return await _unitOfWork.MusicRepository.GetMusicsByQueryAsync(query);
        }

        public async Task<IEnumerable<Music>> FindMusicByIdsAsync(List<string> ids) 
        {
            return await _unitOfWork.MusicRepository.GetMusicsByIdsAsync(ids);
        }

        public async Task<IEnumerable<Music>>  FindDetailedFavoriteMusicsByListenerIdAsync(string listenerId) 
        {
            return await _unitOfWork.MusicRepository.GetDetailedFavoriteMusicsByListenerIdAsync(listenerId);
        }

        public async Task<IEnumerable<Playlist>> FindPlaylistsByQueryAsync(string query)
        {
            return await _unitOfWork.PlaylistRepository.GetPlaylistsByQueryAsync(query);
        }

        public async Task<IEnumerable<Playlist>> FindPlaylistsByListenerIdAsync(string listenerId) 
        {
            return await _unitOfWork.PlaylistRepository.GetPlaylistsWithMusicsByListenerIdAsync(listenerId);
        }
        
        public async Task<Music> FindDetailedMusicAsync(string musicId)
        {
            return await _unitOfWork.MusicRepository.GetDetailedMusicByIdAsync(musicId);
        }

        public async Task<Playlist> FindPlaylistByIdAsync(string playlistId)
        {
            return await _unitOfWork.PlaylistRepository.GetPlaylistByIdAsync(playlistId);
        }
    }
}
