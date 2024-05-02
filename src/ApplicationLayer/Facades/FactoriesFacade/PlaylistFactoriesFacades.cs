using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class PlaylistFactoriesFacades
    {
        private readonly ViewModelFactory _viewModelFactory;
        private readonly ModelFactory _modelFactory;

        public PlaylistFactoriesFacades(ViewModelFactory viewModelFactory, ModelFactory modelFactory) 
        {
            _viewModelFactory = viewModelFactory;
            _modelFactory = modelFactory;
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(Listener listener) 
        {
            return await _viewModelFactory.FacSearchMusicVMAsync(listener);
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(List<string> foundMusicsIds, Listener listener) 
        {
            return await _viewModelFactory.FacSearchMusicVMAsync(foundMusicsIds, listener);
        }

        public IEnumerable<PlaylistMusic> FacPlaylistMusics(string playlistId, IEnumerable<string> musicIds) 
        {
            return _modelFactory.FacPlaylistMusics(playlistId, musicIds);
        }
    }
}
