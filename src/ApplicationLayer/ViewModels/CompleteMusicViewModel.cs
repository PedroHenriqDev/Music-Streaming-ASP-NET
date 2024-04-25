using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels
{
    public class CompleteMusicViewModel
    {
        public Music Music { get; set; }
        public MusicData MusicData { get; set; }
        public string DurationText {  get; set; }

        public CompleteMusicViewModel() 
        { 
        }

        public CompleteMusicViewModel(Music music, MusicData musicData, string durationText) 
        {
            Music = music;
            MusicData = musicData;
            DurationText = durationText;
        }
    }
}
