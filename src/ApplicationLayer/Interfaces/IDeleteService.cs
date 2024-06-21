using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface IDeleteService
{
    Task DeleteEntityByIdAsync<T>(string id)
        where T : class, IEntity;

    Task DeleteFavoriteMusicAsync(string musicId, string listenerId);

    Task DeleteFavoritePlaylistAsync(string playlistId, string listenerId);

    Task<EntityQuery<PlaylistMusic>> DeletePlaylistMusicAsync(PlaylistMusic playlistMusic);
}
