using ApplicationLayer.Interfaces;
using DataAccessLayer.UnitOfWork;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Services;

public class DeleteService : IDeleteService
{
    private readonly ILogger<DeleteService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVerifyService _verifyService;

    public DeleteService(ILogger<DeleteService> logger, IUnitOfWork unitOfWork, IVerifyService verifyService)
    {
        _logger = logger;
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

    public async Task<EntityQuery<FavoriteMusic>> DeleteFavoriteMusicAsync(FavoriteMusic favoriteMusic)
    {
        try
        {
            await _unitOfWork.MusicRepository.RemoveFavoriteMusicAsync(favoriteMusic);
            return new EntityQuery<FavoriteMusic>(true, "Deleted successfully", favoriteMusic, DateTime.Now);
        }
        catch(Exception ex) 
        {
            _logger.LogError($"Error in delete favorite music, because {ex.Message}");
            return new EntityQuery<FavoriteMusic>(false, $"Deletion error, Status Code:{StatusCodes.Status500InternalServerError}", favoriteMusic, DateTime.Now);
        }
    }

    public async Task<EntityQuery<FavoritePlaylist>> DeleteFavoritePlaylistAsync(FavoritePlaylist favoritePlaylist) 
    {
        try
        {
            await _unitOfWork.PlaylistRepository.RemoveFavoritePlaylistAsync(favoritePlaylist);
            return new EntityQuery<FavoritePlaylist>(true, "Deleted succesfully", favoritePlaylist, DateTime.Now);
        }
        catch(Exception ex) 
        {
            _logger.LogError($"Error ocurred in method: {DeleteFavoritePlaylistAsync}, Error: {ex.Message}");
            return new EntityQuery<FavoritePlaylist>(false, $"Deletion error, Status Code: {StatusCodes.Status500InternalServerError}", favoritePlaylist, DateTime.Now);
        }
    }

    public async Task<EntityQuery<PlaylistMusic>> DeletePlaylistMusicAsync(PlaylistMusic playlistMusic)
    {
        try
        {
            await _unitOfWork.EntitiesAssociationRepository.RemovePlaylistMusicAsync(playlistMusic);
            return new EntityQuery<PlaylistMusic>(true, "Deleted successfully", playlistMusic, DateTime.Now);
        }
        catch(Exception ex)
        {
            _logger.LogError($"Error ocurred in method: {DeletePlaylistMusicAsync}, Error: {ex.Message}");
            return new EntityQuery<PlaylistMusic>(false, $"Deletion error, Status Code:{StatusCodes.Status500InternalServerError}", playlistMusic, DateTime.Now);
        }
    }
}

