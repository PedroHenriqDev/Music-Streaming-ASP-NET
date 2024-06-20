using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface ISearchService
{

    Task<T> FindUserByNameAsync<T>(string name)
        where T : class, IUser<T>;

    Task<T> FindUserByIdAsync<T>(string id)
        where T : class, IUser<T>;


    Task<T> FindEntityByIdAsync<T>(string id)
        where T : class, IEntity;


    Task<IEnumerable<T>> FindEntitiesByFKAsync<T, TR>(string fkId)
        where T : class, IEntity where TR : class, IEntity;


    Task<T> FindCurrentUserAsync<T>()
        where T : class, IUser<T>;

   Task<IEnumerable<Genre>> FindUserGenresAsync<T>(string userId)
        where T : class, IUser<T>;

    Task<IEnumerable<T>> FindAllEntitiesAsync<T>()
        where T : class, IEntity;


    Task<T> FindEntityByEmailAsync<T>(string email)
        where T : IEntityWithEmail<T>;

    Task<T> FindUserByCredentialsAsync<T>(string email, string password)
        where T : IUser<T>;

    Task<IEnumerable<Music>> FindMusicsByFkIdsAsync<T>(IEnumerable<string> fkIds)
        where T : class, IEntity;

    Task<IEnumerable<Music>> FindMusicByFkIdAsync<T>(string fkId)
        where T : class, IEntity;

    Task<IEnumerable<Music>> FindMusicsByQueryAsync(string query);

    Task<IEnumerable<Music>> FindMusicByIdsAsync(List<string> ids);

    Task<IEnumerable<Music>> FindDetailedFavoriteMusicsByListenerIdAsync(string listenerId);

    Task<IEnumerable<Playlist>> FindPlaylistsByQueryAsync(string query, string listenerId);

    Task<IEnumerable<Playlist>> FindPlaylistsByListenerIdAsync(string listenerId);

    Task<Music> FindDetailedMusicAsync(string musicId);

    Task<Playlist> FindPlaylistByIdAsync(string playlistId);
}
