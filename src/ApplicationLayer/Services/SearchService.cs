using DataAccessLayer.Repositories;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Services
{
    public class SearchService
    {
        private readonly IHttpContextAccessor _httpAcessor;
        private readonly GenericRepository _genericRepository;
        private readonly UserRepository _userRepository;
        private readonly EntitiesAssociationRepository _associationRepository;
        private readonly MusicRepository _musicRepository;
        private readonly PlaylistRepository _playlistRepository;

        public SearchService(IHttpContextAccessor httpAcessor,
                             GenericRepository genericRepository,
                             UserRepository userRepository, 
                             EntitiesAssociationRepository associationRepository,
                             MusicRepository musicRepository,
                             PlaylistRepository playlistRepository)
        {
            _httpAcessor = httpAcessor;
            _genericRepository = genericRepository;
            _userRepository = userRepository;
            _associationRepository = associationRepository;
            _musicRepository = musicRepository;
            _playlistRepository = playlistRepository;
        }

        public async Task<T> FindUserByNameAsync<T>(string name)
            where T : class, IUser<T>
        {
            return await _userRepository.GetUserByNameAsync<T>(name);
        }

        public async Task<T> FindUserByIdAsync<T>(string id)
            where T : class, IUser<T>
        {
            return await _userRepository.GetUserByIdAsync<T>(id);
        }

        public async Task<T> FindEntityByIdAsync<T>(string id)
            where T : class, IEntity
        {
            return await _genericRepository.GetEntityByIdAsync<T>(id);
        }

        public async Task<IEnumerable<T>> FindEntitiesByFKAsync<T, TR>(string fkId) 
            where T : class, IEntity where TR : class, IEntity 
        {
            return await _genericRepository.GetEntitiesByFKAsync<T, TR>(fkId);
        }

        public async Task<T> FindCurrentUserAsync<T>()
            where T : class, IUser<T>
        {
            return await _userRepository.GetUserByNameAsync<T>(_httpAcessor.HttpContext.User.Identity.Name);
        }

        public async Task<IEnumerable<Genre>> FindUserGenresAsync<T>(string userId)
            where T : class, IUser<T>
        {
            var userGenres = await _associationRepository.GetUserGenresAsync<T>(userId);
            return await _genericRepository.GetEntitiesByIdsAsync<Genre>(userGenres.Select(g => g.GenreId).ToList());
        }

        public async Task<IEnumerable<T>> FindAllEntitiesAsync<T>()
            where T : class, IEntity
        {
            return await _genericRepository.GetAllEntitiesAsync<T>();
        }

        public async Task<T> FindEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            if (email == null) throw new SearchException("Email reference null");

            return await _genericRepository.GetEntityByEmailAsync<T>(email);
        }

        public async Task<T> FindUserByCredentialsAsync<T>(string email, string password)
            where T : IUser<T>
        {
            if (password == null || email == null) throw new SearchException("Password or email were used as a null reference");

            return await _genericRepository.GetEntityByCredentialsAsync<T>(email, password);
        }

        public async Task<IEnumerable<Music>> FindMusicsByFkIdsAsync<T>(IEnumerable<string> fkIds) 
            where T : class, IEntity
        {
            return await _musicRepository.GetMusicsByFkIdsAsync<T>(fkIds);
        }

        public async Task<IEnumerable<Music>> FindMusicByFkIdAsync<T>(string fkId) 
            where T : class, IEntity
        {
            return await _musicRepository.GetMusicsByFkIdAsync<T>(fkId);
        }

        public async Task<IEnumerable<Music>> FindMusicsByQueryAsync(string query) 
        {
            return await _musicRepository.GetMusicsByQueryAsync(query);
        }

        public async Task<IEnumerable<Music>> FindMusicByIdsAsync(List<string> ids) 
        {
            return await _musicRepository.GetMusicsByIdsAsync(ids);
        }

        public async Task<IEnumerable<Music>>  FindDetailedFavoriteMusicsByListenerIdAsync(string listenerId) 
        {
            return await _musicRepository.GetDetailedFavoriteMusicsByListenerIdAsync(listenerId);
        }

        public async Task<IEnumerable<Playlist>> FindPlaylistsByQueryAsync(string query)
        {
            return await _playlistRepository.GetPlaylistsByQueryAsync(query);
        }

        public async Task<IEnumerable<Playlist>> FindPlaylistsByListenerIdAsync(string listenerId) 
        {
            return await _playlistRepository.GetPlaylistsWithMusicsByListenerIdAsync(listenerId);
        }
        
        public async Task<Music> FindMusicAsync(string musicId)
        {
            return await _musicRepository.GetMusicByIdAsync(musicId);
        }

        public async Task<Playlist> FindPlaylistByIdAsync(string playlistId)
        {
            return await _playlistRepository.GetPlaylistByIdAsync(playlistId);
        }
    }
}
