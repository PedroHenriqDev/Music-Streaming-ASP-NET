using ApplicationLayer.ViewModels;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface IVerifyService
{
    Task<bool> HasNameInDbAsync<T>(string name)
        where T : class, IUser<T>;

    Task<bool> HasEmailInDbAsync<T>(string email)
        where T : class, IEntityWithEmail<T>;

    Task VerifyDuplicateNameOrEmailAsync(string name, string email);

    Task<bool> HasEntityInDbAsync<T>(string id) 
        where T : class, IEntity;

    EntityVerify<PlaylistViewModel> VefifyPlaylistVM(PlaylistViewModel playlistVM);

    bool VerifyUserGenres(RegisterUserViewModel userVM);

    bool VerifyUser(RegisterUserViewModel userVM);

    bool VerifyMusic(AddMusicViewModel musicVM);

    IEnumerable<MusicViewModel> MarkMusicsViewModelAsFavorite(IEnumerable<FavoriteMusic> favoriteMusics, IEnumerable<MusicViewModel> musicsViewModel);
}
