using DataAccessLayer.Validations;
using DomainLayer.Entities;

namespace DataAccessLayer.Mappers
{
    public class DataMapper
    {
        private readonly DataValidation _dataValidation;

        public DataMapper(DataValidation dataValidation) 
        {
            _dataValidation = dataValidation;
        }

        public Music MapMusic(Music music, Artist artist)
        {
           _dataValidation.ValidateArtistObject(artist);
            music.Artist = artist;
            return music;
        }

        public Playlist MapPlaylistDictionary(Playlist playlist, Dictionary<string, Playlist> playlistDictionary, Music music, Listener listener)
        {
            _dataValidation.ValidatePlaylistObject(playlist, music, listener);

            if (!playlistDictionary.TryGetValue(playlist.Id, out var playlistEntry))
            {
                playlistEntry = playlist;
                playlistEntry.Listener = listener;
                playlistEntry.Musics = new List<Music>();
                playlistDictionary.Add(playlistEntry.Id, playlistEntry);
            }

            ((List<Music>)playlistEntry.Musics).Add(music);
            return playlistEntry;
        }
    }
}
