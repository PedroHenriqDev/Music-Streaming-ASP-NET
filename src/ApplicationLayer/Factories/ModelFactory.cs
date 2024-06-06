using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using Microsoft.Extensions.Logging;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Factories
{
    public class ModelFactory
    {

        private readonly ILogger<ModelFactory> _logger;

        public ModelFactory(ILogger<ModelFactory> logger) 
        {
            _logger = logger;
        }

        public T FacUser<T>(string userId, string description)
            where T : class, IUser<T>, new()
        {
            return new T
            {
                Id = userId,
                Description = description
            };
        }

        public List<UserGenre<T>> FacUserGenres<T>(string userIdid, List<string> genreIds)
            where T : class, IUser<T>
        {
            return genreIds.Select(genreId => new UserGenre<T> 
            {
                Id = userIdid, GenreId = genreId 
            }).ToList();
        }

        public async Task<Music> FacMusicAsync(AddMusicViewModel musicVM, Artist artist, string id)
        {
            if (artist is null)
            {
                _logger.LogWarning("Error occurred due to a null user reference");
                throw new ArgumentNullException("Error occurred when searching for the user");
            }

            return new Music(id, musicVM.Name, artist.Id, musicVM.GenreId, musicVM.Date, DateTime.Now, MusicHelper.GetDuration(musicVM.AudioFile));
        }

        public MusicView FacMusicView(string id, string listenerId, string musicId, DateTime createdAt) 
        {
            return new MusicView(id, listenerId, musicId, createdAt);
        }

        public async Task<MusicData> FacMusicDataAsync(AddMusicViewModel musicVM, string Id)
        {
            byte[] audioBytes = await ByteConvertHelper.ConvertIFormFileInByteAsync(musicVM.AudioFile);
            byte[] pictureBytes = await ByteConvertHelper.ConvertIFormFileInByteAsync(musicVM.PictureFile);
            return new MusicData(Id, audioBytes, pictureBytes);
        }

        public async Task<Playlist> FacPlaylistAsync(PlaylistViewModel playlistVM, string listenerId) 
        {
            return new Playlist(playlistVM.Id, playlistVM.Visibility, listenerId, playlistVM.Name, playlistVM.Description, await ByteConvertHelper.ConvertIFormFileInByteAsync(playlistVM.FileImage), DateTime.Now);
        }

        public IEnumerable<PlaylistMusic> FacPlaylistMusics(string playlistId, string listenerId, IEnumerable<string> musicsIds) 
        {
            return musicsIds.Select(musicId => new PlaylistMusic 
            {
                Id = Guid.NewGuid().ToString(),
                PlaylistId = playlistId,
                ListenerId = listenerId,
                MusicId = musicId
            });
        }

        public FavoriteMusic FacFavoriteMusic(string id, string musicId, string listenerId) 
        {
            return new FavoriteMusic(id, musicId, listenerId);
        }
    }
}
