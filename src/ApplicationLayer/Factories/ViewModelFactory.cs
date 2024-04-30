using ApplicationLayer.ViewModels;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using UtilitiesLayer.Helpers;

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

        public async Task<IEnumerable<CompleteMusicViewModel>> FacCompleteMusicsVMAsync<T>(T user)
           where T : class, IUser<T>
        {
            IEnumerable<Genre> genres = await _searchService.FindUserGenresAsync(user);
            IEnumerable<Music> musics = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(g => g.Id).ToList());
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(m => m.Id).ToList());

            return musics.Join(musicDatas,
                               music => music.Id,
                               musicDatas => musicDatas.Id,
                               (music, musicData) => new CompleteMusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)));
        }

        public ListenerPageViewModel FacListenerPageVM(Listener listener) 
        {
            return new ListenerPageViewModel
            {
                Name = listener.Name,
                Description = listener.Description,
                PictureProfile = listener.PictureProfile,
            };
        }

        public async Task<ArtistPageViewModel> FacArtistPageVMAsync(Artist artist)
        {
            IEnumerable<Music> musics = await _searchService.FindMusicByFkIdAsync<Artist>(artist.Id);
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(m => m.Id).ToList());

            return new ArtistPageViewModel
            {
                Name = artist.Name,
                Description = artist.Description,
                PictureProfile = artist.PictureProfile,
                Musics = musics.Zip(musicDatas, (music, musicData) => new CompleteMusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration))).ToList()
            };
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(Listener listener)
        {
            IEnumerable<Genre> genres = await _searchService.FindUserGenresAsync(listener);
            IEnumerable<Music> musics = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(g => g.Id).ToList());
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(m => m.Id).ToList());

            return new SearchMusics
            {
                MusicsSuggestion = musics.Join(musicDatas,
                                              music => music.Id,
                                              musicData => musicData.Id,
                                              (music, musicData) => new CompleteMusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
            };
        }

        public async Task<SearchMusics> FacSearchMusicVMAsync(List<string> foundMusicsIds, Listener listener)
        {
            IEnumerable<Genre> genres = await _searchService.FindUserGenresAsync(listener);
            IEnumerable<Music> musicsSuggestion = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(m => m.Id).ToList());
            IEnumerable<MusicData> musicSuggestionDatas = await _storageService.DownloadMusicsAsync(musicsSuggestion.Select(m => m.Id).ToList());

            IEnumerable<Music> foundMusics = await _searchService.FindMusicByIdsAsync(foundMusicsIds);
            IEnumerable<MusicData> foundMusicsDatas = await _storageService.DownloadMusicsAsync(foundMusics.Select(m => m.Id).ToList());
                 
            return new SearchMusics
            {
                MusicsSuggestion = musicsSuggestion.Join(musicSuggestionDatas,
                                               music => music.Id,
                                               musicData => musicData.Id, (music, musicData) => new CompleteMusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration))),

                FoundMusics = foundMusics.Join(foundMusicsDatas,
                                               music => music.Id,
                                               musicData => musicData.Id,
                                               (music, musicData) => new CompleteMusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
            };
        }
    }
}