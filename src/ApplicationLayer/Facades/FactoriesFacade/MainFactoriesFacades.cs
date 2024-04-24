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

        public async Task<IEnumerable<CompleteMusicViewModel>> FacCompleteMusicsVMAsync<T>(T user) 
            where T : class, IUser<T> 
        {
            return await _viewModelFactory.FacCompleteMusicsVMAsync(user);
        }
    }
}
