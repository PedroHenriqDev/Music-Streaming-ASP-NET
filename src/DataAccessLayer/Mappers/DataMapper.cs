using DataAccessLayer.Validations;
using DomainLayer.Entities;

namespace DataAccessLayer.Mappers;

public class DataMapper
{
    private readonly DataValidation _dataValidation;

    public DataMapper(DataValidation dataValidation) 
    {
        _dataValidation = dataValidation;
    }

    public Music MapMusicArtist(Music music, Artist artist)
    {
       _dataValidation.ValidateArtistObject(artist);
        music.Artist = artist;
        return music;
    }

    public Music MapMusicViews(Music music, Dictionary<string, Music> musicDictionary, MusicView musicView, Genre genre) 
    {
        _dataValidation.ValidateMusicObject(music, musicView);

        if(!musicDictionary.TryGetValue(music.Id, out var musicEntry))
        {
            musicEntry = music;
            musicEntry.Genre = genre;
            musicEntry.MusicViews = new List<MusicView>();
            musicDictionary.Add(music.Id, musicEntry);
        }

        ((List<MusicView>)musicEntry.MusicViews).Add(musicView);
        return musicEntry;
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
