using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Factories
{
    public class ViewModelFactory
    {
        private readonly GenerateIntelliTextService _generateTextService;

        public ViewModelFactory(GenerateIntelliTextService generateTextService)
        {
            _generateTextService = generateTextService;
        }

        public async Task<DescriptionViewModel> FacListenerDescriptionViewModelAsync(Listener listener)
        {
            if (listener is null)
            {
                throw new ArgumentNullException("It is impossible to manufacture objects that have null properties");
            }

            return new DescriptionViewModel(listener.Description, listener.Name, listener.Id, await _generateTextService.GenerateListenerDescriptionAsync(listener));
        }

        public async Task<DescriptionViewModel> FacArtistDescriptionViewModelAsync(Artist artist)
        {
            if (artist is null)
            {
                throw new ArgumentNullException("It is impossible to manufacture objects that have null properties");
            }

            return new DescriptionViewModel(artist.Description, artist.Name, artist.Id, await _generateTextService.GenerateArtistDescriptionAsync(artist));
        }

    }
}
