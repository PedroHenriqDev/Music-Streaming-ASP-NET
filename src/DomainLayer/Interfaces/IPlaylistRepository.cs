using DomainLayer.Entities;

namespace DomainLayer.Interfaces;

public interface IPlaylistRepository
{
    Task<IEnumerable<Playlist>> GetPlaylistsWithMusicsByListenerIdAsync(string listenerId);

    Task<Playlist> GetPlaylistByIdAsync(string playlistId);

    Task<IEnumerable<Playlist>> GetPlaylistsByQueryAsync(string query, string listenerId);
 
    Task RecordPlaylistAsync(Playlist playlist);

    Task RecordFavoritePlaylistAsync(FavoritePlaylist favoritePlaylist);

    Task RemoveFavoritePlaylistAsync(string playlistId, string listenerId);
}
