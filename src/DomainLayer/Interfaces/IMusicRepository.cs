using DomainLayer.Entities;

namespace DomainLayer.Interfaces
{
    public interface IMusicRepository
    {
        Task<IEnumerable<Music>> GetMusicsByIdsAsync(List<string> ids);

        Task<IEnumerable<Music>> GetMusicsByQueryAsync(string query);

        Task<IEnumerable<Music>> GetMusicsByFkIdAsync<T>(string fkId)
            where T : class, IEntity;

        Task<IEnumerable<Music>> GetMusicsByFkIdsAsync<T>(IEnumerable<string> fkIds)
           where T : class, IEntity;

        Task<IEnumerable<FavoriteMusic>> GetBasicFavoriteMusicsByListenerIdAsync(string listenerId);

        Task<IEnumerable<Music>> GetDetailedFavoriteMusicsByListenerIdAsync(string listenerId);

        Task<Music> GetDetailedMusicByIdAsync(string musicId);

        Task RecordMusicAsync(Music music);

        Task RecordMusicViewAsync(MusicView musicView);

        Task RecordFavoriteMusicAsync(FavoriteMusic favoriteMusic);

        Task RemoveFavoriteMusicAsync(string musicId, string listenerId);
    }
}
