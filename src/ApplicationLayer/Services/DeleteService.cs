using ApplicationLayer.Interfaces;
using DataAccessLayer.UnitOfWork;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
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

    public async Task<EntityQuery<PlaylistMusic>> DeletePlaylistMusicAsync(PlaylistMusic playlistMusic)
    {
        try
        {
            await _unitOfWork.EntitiesAssociationRepository.RemovePlaylistMusicAsync(playlistMusic);
            return new EntityQuery<PlaylistMusic>(true, "Deleted successfully", playlistMusic, DateTime.Now);
        }
        catch(Exception ex)
        {
            _logger.LogError($"Error ocurrer in method: {DeletePlaylistMusicAsync}, Error: {ex.Message}");
            return new EntityQuery<PlaylistMusic>(false, $"Deletion error, because: {ex.Message}", playlistMusic, DateTime.Now);
        }
    }
}

