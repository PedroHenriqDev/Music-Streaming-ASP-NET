using DomainLayer.Entities;

namespace DomainLayer.Interfaces;

public interface IEntitiesAssociationRepository
{
    Task<IEnumerable<UserGenre<T>>> GetUserGenresAsync<T>(string id) where T : class, IUser<T>;
    Task RecordPlaylistMusicsAsync(IEnumerable<PlaylistMusic> playlistMusics);
    Task RecordUserGenresAsync<T>(List<UserGenre<T>> userGenres) where T : class, IUser<T>;
}
