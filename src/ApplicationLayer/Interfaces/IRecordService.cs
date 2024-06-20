using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface IRecordService
{
    Task<EntityQuery<T>> RecordUserAsync<T>(T user)
     where T : class, IUser<T>;

    Task<EntityQuery<List<UserGenre<T>>>> RecordUserGenresAsync<T>(List<UserGenre<T>> userGenres)
        where T : class, IUser<T>;

    Task<EntityQuery<Music>> RecordMusicAsync(AddMusicViewModel musicVM, Artist artist);

    Task<EntityQuery<MusicView>> RecordMusicViewAsync(MusicView musicView);

    Task<EntityQuery<FavoriteMusic>> RecordFavoriteMusicAsync(FavoriteMusic favoriteMusic);
   
    Task<EntityQuery<Playlist>> RecordPlaylistAsync(Playlist playlist);

    Task<EntityQuery<FavoritePlaylist>> RecordFavoritePlaylistAsync(FavoritePlaylist favoritePlaylist);

    Task<EntityQuery<IEnumerable<PlaylistMusic>>> RecordPlaylistMusicsAsync(IEnumerable<PlaylistMusic> playlistMusics);
}
