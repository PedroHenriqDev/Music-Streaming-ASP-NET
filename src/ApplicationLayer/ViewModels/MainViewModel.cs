using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels
{
    public class MainViewModel
    {
        public IEnumerable<MusicViewModel>? MusicsViewModel { get; set; }
        public IEnumerable<FavoriteMusic>? FavoriteMusics { get; set; }

        public MainViewModel() 
        {
        }

        public MainViewModel(IEnumerable<MusicViewModel>? musicsViewModel, IEnumerable<FavoriteMusic>? favoriteMusics)
        {
            MusicsViewModel = musicsViewModel;
            FavoriteMusics = favoriteMusics;
        }
    }
}
