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

        private readonly SearchService _searchService;
        private readonly ILogger<ModelFactory> _logger;

        public ModelFactory(SearchService searchService, ILogger<ModelFactory> logger) 
        {
            _searchService = searchService;
            _logger = logger;
        }

        public T FacUser<T>(string id, string description)
            where T : class, IUser<T>, new()
        {
            return new T
            {
                Id = id,
                Description = description
            };
        }

        public T FacUser<T>(string id, string name, string email, string password, string phoneNumber, DateTime birthDate, DateTime dateCreation)
          where T : class, IUser<T>, new()
        {
            return new T
            {
                Id = id,
                Name = name,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
                BirthDate = birthDate,
                DateCreation = dateCreation
            };
        }

        public List<UserGenre<T>> FacUserGenres<T>(string id, List<string> ids)
            where T : class, IUser<T>
        {
            return ids.Select(genreId => new UserGenre<T> { Id = id, GenreId = genreId }).ToList();
        }

        public async Task<Music> FacMusicAsync(AddMusicViewModel musicVM, string id)
        {
            Artist artist = await _searchService.FindCurrentUserAsync<Artist>();
            if (artist is null)
            {
                _logger.LogWarning("Error occurred due to a null user reference");
                throw new ArgumentNullException("Error occurred when searching for the user");
            }

            return new Music(id, musicVM.Name, artist.Id, musicVM.GenreId, musicVM.Date, DateTime.Now, MusicHelper.GetDuration(musicVM.AudioFile));
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
    }
}
