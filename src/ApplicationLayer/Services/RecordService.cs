using ApplicationLayer.Factories;
using ApplicationLayer.Interfaces;
using ApplicationLayer.ViewModels;
using DataAccessLayer.UnitOfWork;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Services;

public class RecordService : IRecordService
{
    private readonly ILogger<RecordService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DomainFactory _domainCreationService;
    private readonly ICloudStorageService _storageService;

    public RecordService(ILogger<RecordService> logger,
                         IUnitOfWork unitOfWork,
                         DomainFactory domainCreationService, 
                         ICloudStorageService storageService)
    {
        _logger = logger;
        _domainCreationService = domainCreationService;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<EntityQuery<T>> RecordUserAsync<T>(T user) 
        where T : class, IUser<T>
    {
        try
        {
            await _unitOfWork.UserRepository.RecordUserAsync(user);
            return new EntityQuery<T>(true, $"{typeof(T).Name} created succesfully", user, DateTime.Now);
        }
        catch(Exception ex) 
        {
            _logger.LogError($"Brutal error in method CreateUserAsync - Error Message: {ex.Message}");
            return new EntityQuery<T>(false, "Unable to create a artist", user, DateTime.Now);
        }
    }

    public async Task<EntityQuery<List<UserGenre<T>>>> RecordUserGenresAsync<T>(List<UserGenre<T>> userGenres) 
        where T : class, IUser<T>
    {
        try
        {
            await _unitOfWork.EntitiesAssociationRepository.RecordUserGenresAsync(userGenres);
            return new EntityQuery<List<UserGenre<T>>>(true, $"{typeof(T).Name} created succesfully", userGenres, DateTime.Now);
        }
        catch (Exception ex) 
        {
            _logger.LogError($"Brutal error in method CreateUserGenresAsync - Error Message: {ex.Message}");
            throw new RecordException<EntityQuery<List<UserGenre<T>>>>($"This error occurred while registration was happening, {ex.Message}", 
                  new EntityQuery<List<UserGenre<T>>>(false, "Unable to create a artist", userGenres, DateTime.Now));
        }
    }

    public async Task<EntityQuery<Music>> RecordMusicAsync(AddMusicViewModel musicVM, Artist artist)
    {
        string id = Guid.NewGuid().ToString();
        var music = await _domainCreationService.CreateMusicAsync(musicVM, artist, id);
        try 
        {
            await _unitOfWork.MusicRepository.RecordMusicAsync(music);
            await _storageService.UploadMusicAsync(await _domainCreationService.CreateMusicDataAsync(musicVM, music.Id));
            return new EntityQuery<Music>(true, "Create music successfully", music, DateTime.Now);
        }
        catch(Exception ex) 
        {
            if(ex is MusicException) 
            {
                await _unitOfWork.GenericRepository.RemoveEntityByIdAsync<Music>(id);
            }
            return new EntityQuery<Music>(false, $"Unable to create song, because this error: {ex.Message}", music, DateTime.Now);
        }
    }

    public async Task<EntityQuery<MusicView>> RecordMusicViewAsync(MusicView musicView) 
    {
        try 
        {
            await _unitOfWork.MusicRepository.RecordMusicViewAsync(musicView);
            return new EntityQuery<MusicView>(true, "View record successfully", musicView, DateTime.Now);
        }
        catch(Exception ex)
        {
            _logger.LogError($"Error in method 'CreateMusicViewAsync', because: {ex.Message}");
            return new EntityQuery<MusicView>(false, $"Is impossible record this view, because: {ex.Message}", musicView, DateTime.Now);
        }
    }

    public async Task<EntityQuery<FavoriteMusic>> RecordFavoriteMusicAsync(FavoriteMusic favoriteMusic) 
    {
        try
        {
            await _unitOfWork.MusicRepository.RecordFavoriteMusicAsync(favoriteMusic);
            return new EntityQuery<FavoriteMusic>(true, "Favorite music record successfully", favoriteMusic, DateTime.Now);
        }
        catch(Exception ex) 
        {
            return new EntityQuery<FavoriteMusic>(true, $"An unexpected error ocurred, because: {ex.Message}", favoriteMusic, DateTime.Now);
        }
    }

    public async Task<EntityQuery<Playlist>> RecordPlaylistAsync(Playlist playlist) 
    {
        try     
        {
            await _unitOfWork.PlaylistRepository.RecordPlaylistAsync(playlist);
            return new EntityQuery<Playlist>(true, "Playlist created successfully", playlist, DateTime.Now);
        }
        catch(Exception ex) 
        {
            return new EntityQuery<Playlist>(false, $"Unable to create playlist, because this error: {ex.Message}", playlist, DateTime.Now);
        }
    }

    public async Task<EntityQuery<FavoritePlaylist>> RecordFavoritePlaylistAsync(FavoritePlaylist favoritePlaylist) 
    {
        try
        {
            await _unitOfWork.PlaylistRepository.RecordFavoritePlaylistAsync(favoritePlaylist);
            return new EntityQuery<FavoritePlaylist>(true, "Favorite playlist created successfully", favoritePlaylist, DateTime.Now);
        }
        catch(Exception ex) 
        {
            return new EntityQuery<FavoritePlaylist>(false, $"An unexpected error ocurred: {ex.Message}", favoritePlaylist, DateTime.Now);
        }
    }

    public async Task<EntityQuery<IEnumerable<PlaylistMusic>>> RecordPlaylistMusicsAsync(IEnumerable<PlaylistMusic> playlistMusics) 
    {
        try
        {
            await _unitOfWork.EntitiesAssociationRepository.RecordPlaylistMusicsAsync(playlistMusics);
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
