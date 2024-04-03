using Models.Entities;
using Models.Interfaces;
using Models.Queries;
using Services;
using Utilities.Factories;
using ViewModels;

namespace Facades.Services
{
    public class UserServicesFacade<T> where T : class, IUser<T>
    {
        private readonly RecordUserService _recordUserService;
        private readonly LoginService _loginService;
        private readonly SearchService _searchService;
        private readonly UserAuthenticationService _authenticationService;
        private readonly PictureService _pictureService;
        private readonly VerifyService _verifyService;
        private readonly UpdateService _updateService;
        private readonly UserPageService _userPageService;

        public UserServicesFacade(
            RecordUserService recordUserService,
            LoginService loginService,
            SearchService searchService,
            PictureService pictureService,
            UserAuthenticationService authenticationService,
            VerifyService verifyService,
            UserPageService userPageService,
            UpdateService updateService)
        {
            _recordUserService = recordUserService;
            _loginService = loginService;
            _searchService = searchService;
            _pictureService = pictureService;
            _authenticationService = authenticationService;
            _verifyService = verifyService;
            _userPageService = userPageService;
            _updateService = updateService;
        }

        public async Task<bool> LoginAsync(LoginViewModel loginVM)
        {
            return await _loginService.LoginAsync<T>(loginVM);
        }

        public async Task<TR> FindEntityByEmailAsync<TR>(string email)
            where TR : class, IEntityWithEmail<TR>
        {
            return await _searchService.FindEntityByEmailAsync<TR>(email);
        }

        public async Task SignInUserAsync(T user)
        {
            await _authenticationService.SignInUserAsync(user);
        }

        public async Task SignOutUserAsync()
        {
            await _authenticationService.SignOutUserAsync();
        }

        public ArtistPageViewModel BuildArtistViewModel(Artist artist)
        {
            return _userPageService.BuildArtistViewModel(artist);
        }

        public async Task<T> FindCurrentUserAsync()
        {
            return await _searchService.FindCurrentUserAsync<T>();
        }

        public async Task AddPictureProfileAsync(string imageFile, T user)
        {
            await _pictureService.AddPictureProfileAsync(imageFile, user);
        }

        public async Task VerifyDuplicateNameOrEmailAsync(string name, string email)
        {
            await _verifyService.VerifyDuplicateNameOrEmailAsync(name, email);
        }

        public async Task<IEnumerable<TR>> FindAllEntitiesAsync<TR>()
            where TR : class, IEntity
        {
            return await _searchService.FindAllEntitiesAsync<TR>();
        }

        public async Task<EntityQuery<Artist>> CreateArtistAsync(RegisterUserViewModel artistVM)
        {
            return await _recordUserService.CreateArtistAsync(artistVM);
        }

        public async Task<EntityQuery<Listener>> CreateListenerAsync(RegisterUserViewModel userVM)
        {
            return await _recordUserService.CreateListenerAsync(userVM);
        }

        public async Task UpdateDescriptionAsync<TR>(TR user)
            where TR : class, IEntityWithDescription<TR>
        {
            await _updateService.UpdateDescriptionAsync(user);
        }
    }
}
