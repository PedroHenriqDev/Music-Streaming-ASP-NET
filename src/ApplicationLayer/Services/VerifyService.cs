using ApplicationLayer.ViewModels;
using DomainLayer.Exceptions;
using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Services
{
    public class VerifyService
    {
        private readonly SearchService _searchService;

        public VerifyService(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<bool> HasNameInDbAsync<T>(string name)
            where T : class, IUser<T>
        {
            if (await _searchService.FindUserByNameAsync<T>(name) != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> HasEmailInDbAsync<T>(string email)
            where T : class, IEntityWithEmail<T>
        {
            if (await _searchService.FindEntityByEmailAsync<T>(email) != null)
            {
                return true;
            }
            return false;
        }

        public async Task VerifyDuplicateNameOrEmailAsync(string name, string email)
        {

            if (await HasNameInDbAsync<Listener>(name) || await HasNameInDbAsync<Artist>(name))
            {
                throw new EqualException("This name exist");
            }

            if (await HasEmailInDbAsync<Artist>(email) || await HasEmailInDbAsync<Listener>(email))
            {
                throw new EqualException("Existing email.");
            }
        }

        public async Task<bool> HasEntityInDbAsync<T>(string id) where T : class, IEntity
        {
            return await _searchService.FindEntityByIdAsync<T>(id) != null;
        }

        public EntityVerify<PlaylistViewModel> VefifyPlaylistVM(PlaylistViewModel playlistVM)
        {
            if (string.IsNullOrEmpty(playlistVM.Name))
            {
                return new EntityVerify<PlaylistViewModel>(false, "The playlist must have a name", playlistVM);
            }
            else if (playlistVM.FileImage is null)
            {
                return new EntityVerify<PlaylistViewModel>(false, "The playlist must have a name", playlistVM);
            }
            return new EntityVerify<PlaylistViewModel>(true, "Correct playlist", playlistVM);
        }

        public bool VerifyUserGenres(RegisterUserViewModel userVM)
        {
            return userVM.SelectedGenreIds != null && userVM.SelectedGenreIds.Any();
        }

        public bool VerifyUser(RegisterUserViewModel userVM)
        {
            DateTime birthDate = userVM.BirthDate;
            TimeSpan duration = DateTime.Now.Subtract(birthDate);

            return !string.IsNullOrEmpty(userVM.Name) &&
                   !string.IsNullOrEmpty(userVM.Email) &&
                   !string.IsNullOrEmpty(userVM.Password) &&
                   duration.TotalDays >= 3650;
        }

        public bool VerifyMusic(AddMusicViewModel musicVM)
        {
            if (!string.IsNullOrEmpty(musicVM.Name) && musicVM.Date <= DateTime.Now && !string.IsNullOrEmpty(musicVM.GenreId))
            {
                return true;
            }
            return false;
        }
    }
}
