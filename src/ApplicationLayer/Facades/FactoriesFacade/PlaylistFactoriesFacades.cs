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

        public async Task<SearchMusics> FacSearchMusicsVMAsync(Listener listener) 
        {
            return await _viewModelFactory.FacSearchMusicsVMAsync(listener);
        }
    }
}
