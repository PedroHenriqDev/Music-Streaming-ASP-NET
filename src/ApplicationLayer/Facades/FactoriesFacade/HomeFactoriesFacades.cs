using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class HomeFactoriesFacades
    {
        private readonly ViewModelFactory _viewModelFactory;

        public HomeFactoriesFacades(ViewModelFactory viewModelFactory) 
        {
            _viewModelFactory = viewModelFactory;
        }

        public async Task<DisplayMusicViewModel> FacDisplayMusicVMAsync<T>() 
            where T : class, IUser<T> 
        {
            return await _viewModelFactory.FacDisplayMusicVMAsync<T>();
        }
    }
}
