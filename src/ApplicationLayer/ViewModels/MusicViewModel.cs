using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels;

public class MusicViewModel
{
    public Music Music { get; set; }
    public MusicData MusicData { get; set; }
    public string DurationText {  get; set; }
    public bool IsFavorite { get; set; }

    public MusicViewModel() 
    { 
    }

    public MusicViewModel(Music music, MusicData musicData, string durationText) 
    {
        Music = music;
        MusicData = musicData;
        DurationText = durationText;
    }
}
