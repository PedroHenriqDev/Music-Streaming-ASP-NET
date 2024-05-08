using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class ArtistFactoriesFacade
    {
        private readonly ViewModelFactory _viewModelFactory;
        private readonly ModelFactory _modelFactory;

        public ArtistFactoriesFacade(
            ViewModelFactory viewModelFactory, 
            ModelFactory modelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _modelFactory = modelFactory;
        }

        public async Task<DescriptionViewModel> FacArtistDescriptionVMAsync(Artist artist) 
        {
            return await _viewModelFactory.FacArtistDescriptionVMAsync(artist);
        }

        public async Task<ArtistPageViewModel> FacArtistPageVMAsync(Artist artist) 
        {
            return await _viewModelFactory.FacArtistPageVMAsync(artist);
        }
    }
}
