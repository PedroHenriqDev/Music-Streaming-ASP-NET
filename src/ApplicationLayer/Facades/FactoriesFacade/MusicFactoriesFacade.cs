using ApplicationLayer.Factories;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class MusicFactoriesFacade
    {
        private readonly ModelFactory _modelFactory;
        private readonly ViewModelFactory _viewModelFactory;
        public MusicFactoriesFacade(ModelFactory modelFactory,
                                    ViewModelFactory viewModelFactory)
        {
            _modelFactory = modelFactory;
            _viewModelFactory = viewModelFactory;
        }

        public MusicView FacMusicView(string id, string listenerId, string musicId, DateTime createdAt)
        {
            return _modelFactory.FacMusicView(id, listenerId, musicId, createdAt);
        }

        public async Task<MusicViewModel> FacMusicViewModelAsync(Music music, bool isFavorite)
        {
            return await _viewModelFactory.FacMusicViewModelAsync(music, isFavorite);
        }

        public FavoriteMusic FacFavoriteMusic(string id, string musicId, string listenerId) 
        {
            return _modelFactory.FacFavoriteMusic(id, musicId, listenerId);
        }
    }
}
