using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels
{
    public class DisplayMusicViewModel
    {
        public IEnumerable<Music> Musics { get; set; }
        public IEnumerable<MusicData> MusicDatas { get; set; }
    }
}
