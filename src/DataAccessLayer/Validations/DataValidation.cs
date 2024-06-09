using DomainLayer.Entities;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Validations
{
    public class DataValidation
    {
        private readonly ILogger<DataValidation> _logger;

        public DataValidation(ILogger<DataValidation> logger) 
        {
            _logger = logger;
        }

        public void ValidatePlaylistObject(Playlist playlist, Music music, Listener listener) 
        {
            if (playlist is null)
            {
                _logger.LogError($"An error ocurred in method {nameof(ValidatePlaylistObject)} - because playlist is null");
                throw new ArgumentNullException(nameof(playlist), "Playlist is null");
            }
            if (music is null)
            {
                _logger.LogError($"An error ocurred in method {nameof(ValidatePlaylistObject)} - because music is null");
                throw new ArgumentNullException(nameof(music), "Music is null");
            }
            if (listener is null)
            {
                _logger.LogError($"An error ocurred in method {nameof(ValidatePlaylistObject)} - because listener is null");
                throw new ArgumentNullException(nameof(listener), "Listener is null");
            }
        }

        public void ValidateMusicObject(Music music, MusicView musicView)
        {
            if (music is null) 
            {
                _logger.LogError($"An error ocurred in method {nameof(ValidateMusicObject)} - because music is null");
                throw new ArgumentNullException(nameof(music), "Music is null");
            }
            if (musicView is null) 
            {
                _logger.LogError($"An error ocurred in method {nameof(ValidateMusicObject)} - because music view is null");
                throw new ArgumentNullException(nameof(musicView), "Music View is null");
            }
        }


        public void ValidateArtistObject(Artist artist) 
        {
            if(artist is null) 
            {
                _logger.LogError($"An error ocurred in method {nameof(ValidateArtistObject)} - because artist is null");
                throw new ArgumentNullException(nameof(artist), "Artist is null");
            }
        }
    }
}
