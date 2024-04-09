using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Factories
{
    public class ViewModelFactory
    {
        private readonly GenerateIntelliTextService _generateTextService;
        private readonly SearchService _searchService;
        private readonly CloudStorageService _storageService;

        public ViewModelFactory(
            GenerateIntelliTextService generateTextService, 
            SearchService searchService,
            CloudStorageService storageService)
        {
            _generateTextService = generateTextService;
            _searchService = searchService;
            _storageService = storageService;
        }

        public async Task<DescriptionViewModel> FacListenerDescriptionVMAsync(Listener listener)
        {
            if (listener is null)
            {
                throw new ArgumentNullException("It is impossible to manufacture objects that have null properties");
            }

            return new DescriptionViewModel(listener.Description, listener.Name, listener.Id, await _generateTextService.GenerateListenerDescriptionAsync(listener));
        }

        public async Task<DescriptionViewModel> FacArtistDescriptionVMAsync(Artist artist)
        {
            if (artist is null)
            {
                throw new ArgumentNullException("It is impossible to manufacture objects that have null properties");
            }

            return new DescriptionViewModel(artist.Description, artist.Name, artist.Id, await _generateTextService.GenerateArtistDescriptionAsync(artist));
        }

        public async Task<DisplayMusicViewModel> FacDisplayMusicVMAsync<T>() 
            where T : class, IUser<T> 
        {
            T user = await _searchService.FindCurrentUserAsync<T>();
            IEnumerable<Genre> genres = await _searchService.FindUserGenresAsync(user);
            IEnumerable<Music> musics = await _searchService.FindMusicsByGenreIdsAsync(genres.Select(g => g.Id).ToList());
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(m => m.Id).ToList());
            return new DisplayMusicViewModel
            {
                Musics = musics,
                MusicDatas = musicDatas
            };
        }
    }
}
