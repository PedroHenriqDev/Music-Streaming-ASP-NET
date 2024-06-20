using DomainLayer.Entities;

namespace ApplicationLayer.ViewModels;

public class SearchPlaylistViewModel
{
    public IEnumerable<PlaylistViewModel>? PlaylistsViewModel { get; set; }
    public IEnumerable<FavoritePlaylist>? FavoritePlaylists { get; set; }
}
