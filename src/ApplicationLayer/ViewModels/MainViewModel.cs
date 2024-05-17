using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels
{
    public class MainViewModel
    {
        public IEnumerable<MusicViewModel> MusicViewModel { get; set; }
        public IEnumerable<FavoriteMusic>? FavoriteMusic { get; set; }
    }
}
