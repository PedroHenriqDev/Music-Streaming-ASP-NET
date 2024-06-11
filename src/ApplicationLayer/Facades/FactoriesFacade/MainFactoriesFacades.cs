using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class MainFactoriesFacades
    {
        private readonly ViewModelFactory _viewModelFactory;

        public MainFactoriesFacades(ViewModelFactory viewModelFactory) 
        {
            _viewModelFactory = viewModelFactory;
        }

        public async Task<IEnumerable<MusicViewModel>> FacMusicsVMAsync<T>(T user) 
            where T : class, IUser<T> 
        {
            return await _viewModelFactory.FacMusicsViewModelByUserIdAsync<T>(user.Id);
        }

        public async Task<MainViewModel> FacMainVMAsync(IEnumerable<MusicViewModel> musicsVM, string listenerId) 
        {
            return await _viewModelFactory.FacMainVMAsync(musicsVM, listenerId);
        }
    }
}
