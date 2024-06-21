using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface IDeleteService
{
    Task DeleteEntityByIdAsync<T>(string id)
        where T : class, IEntity;

    Task<EntityQuery<FavoriteMusic>> DeleteFavoriteMusicAsync(FavoriteMusic favoriteMusic);

    Task<EntityQuery<FavoritePlaylist>> DeleteFavoritePlaylistAsync(FavoritePlaylist favoritePlaylist);

    Task<EntityQuery<PlaylistMusic>> DeletePlaylistMusicAsync(PlaylistMusic playlistMusic);
}
