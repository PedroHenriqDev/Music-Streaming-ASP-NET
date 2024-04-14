using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class UserFactoriesFacade<T> where T : class, IUser<T>, new()
    {

        private readonly ViewModelFactory _viewModelFactory;
        private readonly ModelFactory _modelFactory;

        public UserFactoriesFacade(ViewModelFactory viewModelFactory, ModelFactory modelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _modelFactory = modelFactory;
        }

        public async Task<DescriptionViewModel> FacListenerDescriptionVMAsync(Listener listener)
        {
            return await _viewModelFactory.FacListenerDescriptionVMAsync(listener);
        }

        public async Task<DescriptionViewModel> FacArtistDescriptionVMAsync(Artist artist) 
        {
            return await _viewModelFactory.FacArtistDescriptionVMAsync(artist);
        }

        public async Task<ArtistPageViewModel> FacArtistPageVMAsync(Artist artist) 
        {
            return await _viewModelFactory.FacArtistPageVMAsync(artist);
        }

        public T FacUser(string id, string description)
        {
            return _modelFactory.FacUser<T>(id, description);
        }
    }
}
