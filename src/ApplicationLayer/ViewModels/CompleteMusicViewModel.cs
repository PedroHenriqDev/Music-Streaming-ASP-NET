using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels
{
    public class CompleteMusicViewModel
    {
        public Music Music { get; set; }
        public MusicData MusicData { get; set; }

        public CompleteMusicViewModel() 
        { 
        }

        public CompleteMusicViewModel(Music music, MusicData musicData) 
        {
            Music = music;
            MusicData = musicData;
        }
    }
}
