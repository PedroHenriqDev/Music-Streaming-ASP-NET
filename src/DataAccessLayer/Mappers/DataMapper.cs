using DomainLayer.Entities;
using DomainLayer.Exceptions;

namespace DataAccessLayer.Mappers
{
    public class DataMapper
    {
        public Music MapMusic(Music music, Artist artist)
        {
            if (artist is null)
            {
                throw new QueryException("Error, artist null");
            }
            music.Artist = artist;
            return music;
        }

        public Playlist MapPlaylist(Playlist playlist, Dictionary<string, Playlist> playlistDictionary, Music music, Listener listener)
        {
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
