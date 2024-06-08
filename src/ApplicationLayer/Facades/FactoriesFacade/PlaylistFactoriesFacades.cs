using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class PlaylistFactoriesFacades
    {
        private readonly ViewModelFactory _viewModelFactory;
        private readonly ModelFactory _modelFactory;

        public PlaylistFactoriesFacades(
            ViewModelFactory viewModelFactory,
            ModelFactory modelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _modelFactory = modelFactory;
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(string listenerId)
        {
            return await _viewModelFactory.FacSearchMusicVMAsync(listenerId);
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(List<string> foundMusicsIds, string listenerId)
        {
            return await _viewModelFactory.FacSearchMusicVMAsync(foundMusicsIds, listenerId);
        }

        public IEnumerable<PlaylistMusic> FacPlaylistMusics(string playlistId, string listenerId, IEnumerable<string> musicIds)
        {
            return _modelFactory.FacPlaylistMusics(playlistId, listenerId, musicIds);
        }

        public async Task<Playlist> FacPlaylistAsync(PlaylistViewModel playlistVM, string listenerId)
        {
            return await _modelFactory.FacPlaylistAsync(playlistVM, listenerId);
        }

        public async Task<PlaylistViewModel> FacPlaylistViewModelAsync(Playlist playlist) 
        {
            return await _viewModelFactory.FacPlaylistViewModelAsync(playlist);
        }

        public async Task<IEnumerable<PlaylistViewModel>> FacPlaylistViewModelsAsync(IEnumerable<Playlist> playlists) 
        {
            return await _viewModelFactory.FacPlaylistViewModelsAsync(playlists);
        }
    }
}
