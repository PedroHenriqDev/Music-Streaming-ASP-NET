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
        private readonly ByteConvertHelper _byteHelper;

        public ModelFactory(SearchService searchService, ILogger<ModelFactory> logger, ByteConvertHelper byteHelper) 
        {
            _searchService = searchService;
            _logger = logger;
            _byteHelper = byteHelper;
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
            byte[] audioBytes = await _byteHelper.ConvertIFormFileInByteAsync(musicVM.AudioFile);
            byte[] pictureBytes = await _byteHelper.ConvertIFormFileInByteAsync(musicVM.PictureFile);
            return new MusicData(Id, audioBytes, pictureBytes);
        }

        public async Task<Playlist> FacPlaylistAsync(PlaylistViewModel playlistVM, string id) 
        {
            return new Playlist(id, playlistVM.Name, playlistVM.Description, await _byteHelper.ConvertIFormFileInByteAsync(playlistVM.FileImage), DateTime.Now);
        }
    }
}
