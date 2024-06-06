using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DataAccessLayer.Repositories;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Microsoft.Extensions.Logging;
using System.Management;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Services
{
    public class RecordService
    {
        private readonly ILogger<RecordService> _logger;
        private readonly UserRepository _userRepository;
        private readonly MusicRepository _musicRepository;
        private readonly GenericRepository _genericRepository;
        private readonly EntitiesAssociationRepository _associationRepository;
        private readonly PlaylistRepository _playlistRepository;
        private readonly ModelFactory _modelFactory;
        private readonly CloudStorageService _storageService;

        public RecordService(ILogger<RecordService> logger,
                             ModelFactory modelFactory, 
                             UserRepository userRepository,
                             MusicRepository musicRepository,
                             GenericRepository genericRepository,
                             EntitiesAssociationRepository associationRepository,
                             PlaylistRepository playlistRepository,
                             CloudStorageService storageService)
        {
            _logger = logger;
            _modelFactory = modelFactory;
            _storageService = storageService;
            _userRepository = userRepository;
            _musicRepository = musicRepository;
            _genericRepository = genericRepository;
            _associationRepository = associationRepository;
            _playlistRepository = playlistRepository;
            _storageService = storageService;
        }

        public async Task<EntityQuery<T>> CreateUserAsync<T>(T user) 
            where T : class, IUser<T>
        {
            try
            {
                await _userRepository.RecordUserAsync(user);
                return new EntityQuery<T>(true, $"{typeof(T).Name} created succesfully", user, DateTime.Now);
            }
            catch(Exception ex) 
            {
                _logger.LogError($"Brutal error in method CreateUserAsync - Error Message: {ex.Message}");
                return new EntityQuery<T>(false, "Unable to create a artist", user, DateTime.Now);
            }
        }

        public async Task<EntityQuery<List<UserGenre<T>>>> CreateUserGenresAsync<T>(List<UserGenre<T>> userGenres) 
            where T : class, IUser<T>
        {
            try
            {
                await _associationRepository.RecordUserGenresAsync(userGenres);
                return new EntityQuery<List<UserGenre<T>>>(true, $"{typeof(T).Name} created succesfully", userGenres, DateTime.Now);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Brutal error in method CreateUserGenresAsync - Error Message: {ex.Message}");
                throw new RecordException<EntityQuery<List<UserGenre<T>>>>($"This error occurred while registration was happening, {ex.Message}", 
                      new EntityQuery<List<UserGenre<T>>>(false, "Unable to create a artist", userGenres, DateTime.Now));
            }
        }

        public async Task<EntityQuery<Music>> CreateMusicAsync(AddMusicViewModel musicVM, Artist artist)
        {
            string id = Guid.NewGuid().ToString();
            var music = await _modelFactory.FacMusicAsync(musicVM, artist, id);
            try 
            {
                await _musicRepository.RecordMusicAsync(music);
                await _storageService.UploadMusicAsync(await _modelFactory.FacMusicDataAsync(musicVM, music.Id));
                return new EntityQuery<Music>(true, "Create music successfully", music, DateTime.Now);
            }
            catch(Exception ex) 
            {
                if(ex is MusicException) 
                {
                    await _genericRepository.RemoveEntityByIdAsync<Music>(id);
                }
                return new EntityQuery<Music>(false, $"Unable to create song, because this error: {ex.Message}", music, DateTime.Now);
            }
        }

        public async Task<EntityQuery<MusicView>> CreateMusicViewAsync(MusicView musicView) 
        {
            try 
            {
                await _musicRepository.RecordMusicViewAsync(musicView);
                return new EntityQuery<MusicView>(true, "View record successfully", musicView, DateTime.Now);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error in method 'CreateMusicViewAsync', because: {ex.Message}");
                return new EntityQuery<MusicView>(false, $"Is impossible record this view, because: {ex.Message}", musicView, DateTime.Now);
            }
        }

        public async Task<EntityQuery<FavoriteMusic>> CreateFavoriteMusicAsync(FavoriteMusic favoriteMusic) 
        {
            try
            {
                await _musicRepository.RecordFavoriteMusicAsync(favoriteMusic);
                return new EntityQuery<FavoriteMusic>(true, "Favorite music record successfully", favoriteMusic, DateTime.Now);
            }
            catch(Exception ex) 
            {
                return new EntityQuery<FavoriteMusic>(true, $"An unexpected error ocurred, because: {ex.Message}", favoriteMusic, DateTime.Now);
            }
        }


        public async Task<EntityQuery<Playlist>> CreatePlaylistAsync(Playlist playlist) 
        {
            try     
            {
                await _playlistRepository.RecordPlaylistAsync(playlist);
                return new EntityQuery<Playlist>(true, "Playlist created succesasfully", playlist, DateTime.Now);
            }
            catch(Exception ex) 
            {
                return new EntityQuery<Playlist>(false, $"Unable to create playlist, because this error: {ex.Message}", playlist, DateTime.Now);
            }
        }

        public async Task<EntityQuery<IEnumerable<PlaylistMusic>>> CreatePlaylistMusicsAsync(IEnumerable<PlaylistMusic> playlistMusics) 
        {
            try
            {
                await _associationRepository.RecordPlaylistMusicsAsync(playlistMusics);
                return new EntityQuery<IEnumerable<PlaylistMusic>>(true, "Record musics successfully", playlistMusics, DateTime.Now);
            }
            catch (QueryException ex)
            {
                return new EntityQuery<IEnumerable<PlaylistMusic>>(false, $"{ex.Message}", playlistMusics, DateTime.Now);
            }
            catch (Exception ex) 
            {
                return new EntityQuery<IEnumerable<PlaylistMusic>>(false, $"An unexpected error ocurred: {ex.Message}", playlistMusics, DateTime.Now);
            }
        }
    }
}
