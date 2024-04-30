using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class PlaylistFactoriesFacades
    {
        private readonly ViewModelFactory _viewModelFactory;

        public PlaylistFactoriesFacades(ViewModelFactory viewModelFactory) 
        {
            _viewModelFactory = viewModelFactory;
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(Listener listener) 
        {
            return await _viewModelFactory.FacSearchMusicVMAsync(listener);
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(List<string> foundMusicsIds, Listener listener) 
        {
            return await _viewModelFactory.FacSearchMusicVMAsync(foundMusicsIds, listener);
        }
    }
}
