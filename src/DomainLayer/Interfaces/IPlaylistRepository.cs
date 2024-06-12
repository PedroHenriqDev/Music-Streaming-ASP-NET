using DomainLayer.Entities;

namespace DomainLayer.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Playlist>> GetPlaylistsWithMusicsByListenerIdAsync(string listenerId);
   
        Task<Playlist> GetPlaylistByIdAsync(string playlistId);

        Task<IEnumerable<Playlist>> GetPlaylistsByQueryAsync(string query);
     
        Task RecordPlaylistAsync(Playlist playlist);
    }
}
