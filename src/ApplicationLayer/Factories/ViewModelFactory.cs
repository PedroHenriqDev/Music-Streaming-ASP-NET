using ApplicationLayer.Services;
using ApplicationLayer.ViewModels;
using AutoMapper;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Factories
{
    public class ViewModelFactory
    {
        private readonly IMapper _mapper;
        private readonly GenerateIntelliTextService _generateTextService;
        private readonly SearchService _searchService;
        private readonly CloudStorageService _storageService;
        private readonly VerifyService _verifyService;

        public ViewModelFactory(IMapper mapper,
                                GenerateIntelliTextService generateTextService,
                                SearchService searchService,
                                CloudStorageService storageService,
                                VerifyService verifyService)
        {
            _mapper = mapper;
            _generateTextService = generateTextService;
            _searchService = searchService;
            _storageService = storageService;
            _verifyService = verifyService;
        }

        public async Task<DescriptionViewModel> CreateListenerDescriptionViewModelAsync(Listener listener)
        {
            if (listener is null)
            {
                throw new ArgumentNullException("It is impossible to manufacture objects that have null properties");
            }

            var viewModel = _mapper.Map<DescriptionViewModel>(listener);
            viewModel.GeneratedDescription = await _generateTextService.GenerateListenerDescriptionAsync(listener);
            return viewModel;
        }

        public async Task<DescriptionViewModel> CreateArtistDescriptionViewModelAsync(Artist artist)
        {
            if (artist is null)
            {
                throw new ArgumentNullException("It is impossible to manufacture objects that have null properties");
            }

            var viewModel = _mapper.Map<DescriptionViewModel>(artist);
            viewModel.GeneratedDescription = await _generateTextService.GenerateArtistDescriptionAsync(artist);
            return viewModel;
        }

        public async Task<SearchPlaylistViewModel> CreateSearchPlaylistViewModelAsync(IEnumerable<Playlist> playlists, string listenerId)
        {
            var playlistsViewModel = _mapper.Map<List<PlaylistViewModel>>(playlists);
            foreach (var playlistVM in playlistsViewModel)
            {
                var playlist = playlists.First(p => p.Id == playlistVM.Id);
                var musicDataDictionary = (await Task.WhenAll(playlist.Musics.Select(m => _storageService.DownloadMusicAsync(m.Id))))
                    .ToDictionary(musicData => musicData.Id);

                playlistVM.Musics = playlist.Musics.Select(music =>
                {
                    var musicVM = _mapper.Map<MusicViewModel>(music);
                    musicVM.MusicData = musicDataDictionary[music.Id];
                    musicVM.Music = music;
                    return musicVM;
                }).ToList();
            }

            var favoritePlaylists = await _searchService.FindEntitiesByFKAsync<FavoritePlaylist, Listener>(listenerId);
            return new SearchPlaylistViewModel
            {
                PlaylistsViewModel = playlistsViewModel,
                FavoritePlaylists = favoritePlaylists,
            };
        }

        public async Task<MusicViewModel> CreateMusicViewModelAsync(Music music, bool isFavorite)
        {
            var musicData = await _storageService.DownloadMusicAsync(music.Id);
            var musicViewModel = _mapper.Map<MusicViewModel>(music);
            musicViewModel.Music = music;
            musicViewModel.MusicData = musicData;
            musicViewModel.IsFavorite = isFavorite;
            return musicViewModel;
        }

        public async Task<MainViewModel> CreateMainViewModelAsync(IEnumerable<MusicViewModel> musicsViewModel, string listenerId)
        {
            var favoriteMusics = await _searchService.FindEntitiesByFKAsync<FavoriteMusic, Listener>(listenerId);
            var musicsMarkViewModel = _verifyService.MarkMusicsViewModelAsFavorite(favoriteMusics, musicsViewModel);
            return new MainViewModel(musicsMarkViewModel, favoriteMusics);
        }

        public async Task<ListenerPageViewModel> CreateListenerPageViewModelAsync(Listener listener)
        {
            var favoriteMusics = await _searchService.FindDetailedFavoriteMusicsByListenerIdAsync(listener.Id);
            var musicDatas = await _storageService.DownloadMusicsAsync(favoriteMusics.Select(music => music.Id).ToList());

            var musicDataDict = musicDatas.ToDictionary(musicData => musicData.Id);
            return new ListenerPageViewModel
            {
                Name = listener.Name,
                Description = listener.Description,
                PictureProfile = listener.PictureProfile,
                FavoriteMusics = favoriteMusics.Select(music =>
                {
                    var musicVM = _mapper.Map<MusicViewModel>(music);
                    musicVM.MusicData = musicDataDict[music.Id];
                    musicVM.Music = music;
                    return musicVM;
                })
            };
        }

        public async Task<IEnumerable<MusicViewModel>> CreateMusicsViewModelByUserIdAsync<T>(string userId) where T : class, IUser<T>
        {
            var genres = await _searchService.FindUserGenresAsync<T>(userId);
            var musics = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(genre => genre.Id).ToList());
            var musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(music => music.Id).ToList());

            var musicDataDict = musicDatas.ToDictionary(musicData => musicData.Id);
            return musics.Select(music =>
            {
                var musicVM = _mapper.Map<MusicViewModel>(music);
                musicVM.MusicData = musicDataDict[music.Id];
                musicVM.Music = music;
                return musicVM;
            });
        }

        public async Task<ArtistPageViewModel> CreateArtistPageViewModelAsync(Artist artist)
        {
            var musics = await _searchService.FindMusicByFkIdAsync<Artist>(artist.Id);
            var musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(music => music.Id).ToList());

            var musicDataDict = musicDatas.ToDictionary(musicData => musicData.Id);
            return new ArtistPageViewModel
            {
                Name = artist.Name,
                Description = artist.Description,
                PictureProfile = artist.PictureProfile,
                Musics = musics.Select(music =>
                {
                    var musicVM = _mapper.Map<MusicViewModel>(music);
                    musicVM.MusicData = musicDataDict[music.Id];
                    musicVM.Music = music;
                    return musicVM;
                }).ToList()
            };
        }

        public async Task<SearchMusicsViewModel> CreateSearchMusicViewModelAsync(string listenerId)
        {
            var genres = await _searchService.FindUserGenresAsync<Listener>(listenerId);
            var musics = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(genre => genre.Id).ToList());
            var musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(music => music.Id).ToList());

            var musicDataDict = musicDatas.ToDictionary(musicData => musicData.Id);
            return new SearchMusicsViewModel
            {
                MusicsSuggestion = musics.Select(music =>
                {
                    var musicVM = _mapper.Map<MusicViewModel>(music);
                    musicVM.MusicData = musicDataDict[music.Id];
                    musicVM.Music = music;
                    return musicVM;
                }).ToList()
            };
        }

        public async Task<IEnumerable<MusicViewModel>> CreateMusicViewModelByGenreAsync(string userId)
        {
            var genres = await _searchService.FindUserGenresAsync<Listener>(userId);
            var musics = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(genre => genre.Id).ToList());
            var musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(music => music.Id).ToList());

            var musicDataDict = musicDatas.ToDictionary(musicData => musicData.Id);
            return musics.Select(music =>
            {
                var musicVM = _mapper.Map<MusicViewModel>(music);
                musicVM.MusicData = musicDataDict[music.Id];
                musicVM.Music = music;
                return musicVM;
            });
        }

        public async Task<SearchMusicsViewModel> CreateSearchMusicViewModelAsync(List<string> foundMusicsIds, string listenerId)
        {
            var genres = await _searchService.FindUserGenresAsync<Listener>(listenerId);
            var musicsSuggestion = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(genre => genre.Id).ToList());
            var musicSuggestionDatas = await _storageService.DownloadMusicsAsync(musicsSuggestion.Select(music => music.Id).ToList());

            var musicSuggestionDataDict = musicSuggestionDatas.ToDictionary(musicData => musicData.Id);

            var foundMusics = await _searchService.FindMusicByIdsAsync(foundMusicsIds);
            var foundMusicsDatas = await _storageService.DownloadMusicsAsync(foundMusics.Select(music => music.Id).ToList());

            var foundMusicDataDict = foundMusicsDatas.ToDictionary(musicData => musicData.Id);
            return new SearchMusicsViewModel
            {
                MusicsSuggestion = musicsSuggestion.Select(music =>
                {
                    var musicVM = _mapper.Map<MusicViewModel>(music);
                    musicVM.MusicData = musicSuggestionDataDict[music.Id];
                    musicVM.Music = music;
                    return musicVM;
                }).ToList(),

                FoundMusics = foundMusics.Select(music =>
                {
                    var musicVM = _mapper.Map<MusicViewModel>(music);
                    musicVM.MusicData = foundMusicDataDict[music.Id];
                    return musicVM;
                }).ToList()
            };
        }

        public async Task<PlaylistViewModel> CreatePlaylistViewModelAsync(Playlist playlist)
        {
            var musicDatas = await _storageService.DownloadMusicsAsync(playlist.Musics.Select(music => music.Id));
            var musicDataDict = musicDatas.ToDictionary(musicData => musicData.Id);

            var viewModel = _mapper.Map<PlaylistViewModel>(playlist);
            viewModel.Musics = playlist.Musics.Select(music =>
            {
                var musicVM = _mapper.Map<MusicViewModel>(music);
                musicVM.MusicData = musicDataDict[music.Id];
                musicVM.Music = music;
                return musicVM;
            }).ToList();

            return viewModel;
        }

        public async Task<IEnumerable<PlaylistViewModel>> CreatePlaylistsViewModelAsync(IEnumerable<Playlist> playlists)
        {
            var playlistsViewModel = _mapper.Map<List<PlaylistViewModel>>(playlists);

            var allMusicsIds = playlists.SelectMany(playlist => playlist.Musics.Select(music => music.Id)).Distinct();
            var musicDataDictionary = (await Task.WhenAll(allMusicsIds.Select(id => _storageService.DownloadMusicAsync(id))))
                .ToDictionary(musicData => musicData.Id);

            foreach (var playlistVM in playlistsViewModel)
            {
                var playlist = playlists.First(playlist => playlist.Id == playlistVM.Id);
                playlistVM.Musics = playlist.Musics.Select(music =>
                {
                    var musicVM = _mapper.Map<MusicViewModel>(music);
                    musicVM.MusicData = musicDataDictionary[music.Id];
                    musicVM.Music = music;
                    return musicVM;
                }).ToList();
            }

            return playlistsViewModel;
        }
    }
}
