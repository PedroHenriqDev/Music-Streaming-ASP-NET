namespace ApplicationLayer.ViewModels
{
    public class ArtistPageViewModel
    {
        public string Name { get; set; }
        public IEnumerable<MusicViewModel> Musics { get; set; }
        public string Description { get; set; }
        public byte[] PictureProfile { get; set; }
    }
}
