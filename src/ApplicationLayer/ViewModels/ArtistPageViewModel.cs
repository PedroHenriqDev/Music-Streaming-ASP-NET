namespace ApplicationLayer.ViewModels
{
    public class ArtistPageViewModel
    {
        public string Name { get; set; }
        public IEnumerable<CompleteMusicViewModel> Musics { get; set; }
        public string Description { get; set; }
        public byte[] PictureProfile { get; set; }
    }
}
