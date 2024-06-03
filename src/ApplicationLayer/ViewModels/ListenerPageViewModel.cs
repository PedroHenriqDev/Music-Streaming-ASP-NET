namespace ApplicationLayer.ViewModels
{
    public class ListenerPageViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] PictureProfile { get; set; }
        public IEnumerable<MusicViewModel>? FavoriteMusics { get; set; }

        public ListenerPageViewModel() 
        {
        }

        public ListenerPageViewModel(string name, string description, byte[] pictureProfile, IEnumerable<MusicViewModel>? favoriteMusics) 
        {
            Name = name;
            Description = description;
            PictureProfile = pictureProfile;
            FavoriteMusics = favoriteMusics;
        }
    }
}
