namespace ApplicationLayer.ViewModels
{
    public class DisplayMusicViewModel
    {
        public IEnumerable<CompleteMusicViewModel> CompleteMusics { get; set; }

        public DisplayMusicViewModel(IEnumerable<CompleteMusicViewModel> completeMusics)
        {
            CompleteMusics = completeMusics;
        }

        public DisplayMusicViewModel()
        {
        }
    }
}
