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

        public async Task<SearchPlaylistViewModel> FacSearchPlaylistViewModelAsync(IEnumerable<Playlist> playlists, string listenerId)
        {
            var playlistsViewModel = FacPlaylistsViewModelAsync(playlists);
            var favoritePlaylists = await _searchService.FindEntitiesByFKAsync<FavoritePlaylist, Listener>(listenerId);
            return new SearchPlaylistViewModel
            {
                PlaylistsViewModel = playlistsViewModel as IEnumerable<PlaylistViewModel>,
                FavoritePlaylists = favoritePlaylists,
            };
        }

        public async Task<IEnumerable<MusicViewModel>> FacMusicsViewModelByUserIdAsync<T>(string userId)
           where T : class, IUser<T>
        {
            IEnumerable<Genre> genres = await _searchService.FindUserGenresAsync<T>(userId);
            IEnumerable<Music> musics = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(g => g.Id).ToList());
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(m => m.Id).ToList());

            return musics.Join(musicDatas,
                               music => music.Id,
                               musicDatas => musicDatas.Id,
                               (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)));
        }

        public async Task<MusicViewModel> FacMusicViewModelAsync(Music music, bool isFavorite)
        {
            var musicData = await _storageService.DownloadMusicAsync(music.Id);
            return new MusicViewModel
            {
                DurationText = MusicHelper.FormatMusicDuration(music.Duration),
                MusicData = musicData,
                Music = music,
                IsFavorite = isFavorite
            };
        }

        public async Task<MainViewModel> FacMainVMAsync(IEnumerable<MusicViewModel> musicsVM, string listenerId)
        {
            var favoriteMusics = await _searchService.FindEntitiesByFKAsync<FavoriteMusic, Listener>(listenerId);
            return new MainViewModel(musicsVM, favoriteMusics);
        }

        public async Task<ListenerPageViewModel> FacListenerPageVMAsync(Listener listener)
        {
            IEnumerable<Music> favoriteMusics = await _searchService.FindDetailedFavoriteMusicsByListenerIdAsync(listener.Id);
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(favoriteMusics.Select(m => m.Id).ToList());
            return new ListenerPageViewModel
            {
                Name = listener.Name,
                Description = listener.Description,
                PictureProfile = listener.PictureProfile,
                FavoriteMusics = favoriteMusics.Join(musicDatas, music => music.Id,
                                                                 musicData => musicData.Id,
                                                                 (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
            };
        }

        public async Task<ArtistPageViewModel> FacArtistPageVMAsync(Artist artist)
        {
            IEnumerable<Music> musics = await _searchService.FindMusicByFkIdAsync<Artist>(artist.Id);
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(m => m.Id)
                                                      .ToList());

            return new ArtistPageViewModel
            {
                Name = artist.Name,
                Description = artist.Description,
                PictureProfile = artist.PictureProfile,
                Musics = musics.Zip(musicDatas,
                                   (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
                                   .ToList()
            };
        }

        public async Task<SearchMusicsViewModel> FacSearchMusicVMAsync(string listenerId)
        {
            IEnumerable<Genre> genres = await _searchService.FindUserGenresAsync<Listener>(listenerId);
            IEnumerable<Music> musics = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(g => g.Id).ToList());
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(musics.Select(m => m.Id).ToList());

            return new SearchMusicsViewModel
            {
                MusicsSuggestion = musics.Join(musicDatas,
                                              music => music.Id,
                                              musicData => musicData.Id,
                                              (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
            };
        }

        public async Task<SearchMusicsViewModel> FacSearchMusicVMAsync(List<string> foundMusicsIds, string listenerId)
        {
            IEnumerable<Genre> genres = await _searchService.FindUserGenresAsync<Listener>(listenerId);
            IEnumerable<Music> musicsSuggestion = await _searchService.FindMusicsByFkIdsAsync<Genre>(genres.Select(m => m.Id).ToList());
            IEnumerable<MusicData> musicSuggestionDatas = await _storageService.DownloadMusicsAsync(musicsSuggestion.Select(m => m.Id)
                                                                 .ToList());

            IEnumerable<Music> foundMusics = await _searchService.FindMusicByIdsAsync(foundMusicsIds);
            IEnumerable<MusicData> foundMusicsDatas = await _storageService.DownloadMusicsAsync(foundMusics.Select(m => m.Id).ToList());

            return new SearchMusicsViewModel
            {
                MusicsSuggestion = musicsSuggestion.Join(musicSuggestionDatas,
                                               music => music.Id,
                                               musicData => musicData.Id, (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration))),

                FoundMusics = foundMusics.Join(foundMusicsDatas,
                                               music => music.Id,
                                               musicData => musicData.Id,
                                               (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
            };
        }

        public async Task<PlaylistViewModel> FacPlaylistViewModelAsync(Playlist playlist)
        {
            IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(playlist.Musics.Select(m => m.Id));
            return new PlaylistViewModel
            {
                Id = playlist.Id,
                Description = playlist.Description,
                Image = playlist.Image,
                Name = playlist.Name,
                Listener = playlist.Listener,
                Musics = playlist.Musics.Join(musicDatas,
                                        music => music.Id,
                                        musicData => musicData.Id,
                                        (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
            };
        }

        public async Task<IEnumerable<PlaylistViewModel>> FacPlaylistsViewModelAsync(IEnumerable<Playlist> playlists)
        {
            var playlistsViewModel = new List<PlaylistViewModel>();

            var allMusicsIds = playlists.SelectMany(playlist => playlist.Musics.Select(music => music.Id)).Distinct();
            var musicDataDictionary = (await Task.WhenAll(allMusicsIds.Select(id => _storageService.DownloadMusicAsync(id))))
                .ToDictionary(musicData => musicData.Id, musicData => musicData);

            foreach (var playlist in playlists)
            {
                var musicsViewModel = playlist.Musics.Select(music => new MusicViewModel 
                {
                    Music = music,
                    MusicData = musicDataDictionary[music.Id],
                    DurationText = MusicHelper.FormatMusicDuration(music.Duration)
                }).ToList();

                var playlistViewModel = new PlaylistViewModel
                {
                    Id = playlist.Id,
                    Description = playlist.Description,
                    Image = playlist.Image,
                    Name = playlist.Name,
                    Listener = playlist.Listener,
                    Musics = musicsViewModel
                };

                playlistsViewModel.Add(playlistViewModel);
            }
            return playlistsViewModel;
        }

        public async Task<IEnumerable<PlaylistViewModel>> FacPlaylistViewModelsAsync(IEnumerable<Playlist> playlists)
        {
            List<PlaylistViewModel> playlistViewModels = new List<PlaylistViewModel>();

            foreach (var playlist in playlists)
            {
                IEnumerable<MusicData> musicDatas = await _storageService.DownloadMusicsAsync(playlist.Musics.Select(m => m.Id));
                playlistViewModels.Add(new PlaylistViewModel
                {
                    Id = playlist.Id,
                    Description = playlist.Description,
                    Image = playlist.Image,
                    Name = playlist.Name,
                    Listener = playlist.Listener,
                    Musics = playlist.Musics.Join(musicDatas,
                                            music => music.Id,
                                            musicData => musicData.Id,
                                            (music, musicData) => new MusicViewModel(music, musicData, MusicHelper.FormatMusicDuration(music.Duration)))
                });
            }
            return playlistViewModels;
        }
    }
}