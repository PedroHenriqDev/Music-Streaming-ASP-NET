using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using Microsoft.Extensions.Logging;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Services
{
    public class DomainCreationService
    {
        private readonly ILogger<DomainCreationService> _logger;

        public DomainCreationService(ILogger<DomainCreationService> logger)
        {
            _logger = logger;
        }

        public T CreateUser<T>(string userId, string description)
            where T : class, IUser<T>, new()
        {
            return new T
            {
                Id = userId,
                Description = description
            };
        }

        public List<UserGenre<T>> CreateUserGenres<T>(string userIdid, List<string> genreIds)
            where T : class, IUser<T>
        {
            return genreIds.Select(genreId => new UserGenre<T>
            {
                Id = userIdid,
                GenreId = genreId
            }).ToList();
        }

        public async Task<Music> CreateMusicAsync(AddMusicViewModel musicVM, Artist artist, string id)
        {
            if (artist is null)
            {
                _logger.LogWarning("Error occurred due to a null user reference");
                throw new ArgumentNullException("Error occurred when searching for the user");
            }

            return new Music(id, musicVM.Name, artist.Id, musicVM.GenreId, musicVM.Date, DateTime.Now, MusicHelper.GetDuration(musicVM.AudioFile));
        }

        public MusicView CreateMusicView(string id, string listenerId, string musicId, DateTime createdAt)
        {
            return new MusicView(id, listenerId, musicId, createdAt);
        }

        public async Task<MusicData> CreateMusicDataAsync(AddMusicViewModel musicVM, string Id)
        {
            byte[] audioBytes = await ByteConvertHelper.ConvertIFormFileInByteAsync(musicVM.AudioFile);
            byte[] pictureBytes = await ByteConvertHelper.ConvertIFormFileInByteAsync(musicVM.PictureFile);
            return new MusicData(Id, audioBytes, pictureBytes);
        }

        public async Task<Playlist> CreatePlaylistAsync(PlaylistViewModel playlistVM, string listenerId)
        {
            return new Playlist(playlistVM.Id, playlistVM.Visibility, listenerId, playlistVM.Name, playlistVM.Description, await ByteConvertHelper.ConvertIFormFileInByteAsync(playlistVM.FileImage), DateTime.Now);
        }

        public FavoritePlaylist CreateFavoritePlaylist(string id, string playlistId, string listenerId)
        {
            return new FavoritePlaylist
            {
                PlaylistId = playlistId,
                Id = id,
                ListenerId = listenerId
            };
        }

        public IEnumerable<PlaylistMusic> CreatePlaylistMusics(string playlistId, string listenerId, IEnumerable<string> musicsIds)
        {
            return musicsIds.Select(musicId => new PlaylistMusic
            {
                Id = Guid.NewGuid().ToString(),
                PlaylistId = playlistId,
                ListenerId = listenerId,
                MusicId = musicId
            });
        }

        public FavoriteMusic CreateFavoriteMusic(string id, string musicId, string listenerId)
        {
            return new FavoriteMusic(id, musicId, listenerId);
        }
    }
}
