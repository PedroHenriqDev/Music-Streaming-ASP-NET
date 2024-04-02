using Models.ConcreteClasses;
using Models.Interfaces;
using Services;
using ViewModels;

namespace Facades
{
    public class UserServicesFacade<TR> where TR : class, IUser<TR>
    {
        private readonly RecordUserService _recordUserService;
        private readonly LoginService _loginService;
        private readonly SearchService _searchService;
        private readonly UserAuthenticationService _authenticationService;
        private readonly PictureService _pictureService;
        private readonly JsonSerializationHelper _jsonHelper;
        private readonly VerifyService _verifyService;
        private readonly HttpHelper _httpHelper;
        private readonly UpdateService _updateService;
        private readonly UserPageService _userPageService;

        public UserServicesFacade(
            RecordUserService recordUserService,
            LoginService loginService,
            SearchService searchService,
            PictureService pictureService,
            JsonSerializationHelper jsonHelper,
            UserAuthenticationService authenticationService,
            VerifyService verifyService,
            HttpHelper httpHelper,
            UserPageService userPageService,
            UpdateService updateService)
        {
            _recordUserService = recordUserService;
            _loginService = loginService;
            _searchService = searchService;
            _pictureService = pictureService;
            _jsonHelper = jsonHelper;
            _authenticationService = authenticationService;
            _verifyService = verifyService;
            _httpHelper = httpHelper;
            _userPageService = userPageService;
            _updateService = updateService;
        }

        public async Task<bool> LoginAsync(LoginViewModel loginVM)
        {
            return await _loginService.LoginAsync<TR>(loginVM);
        }

        public async Task<T> FindEntityByEmailAsync<T>(string email)
            where T : class, IEntityWithEmail<T>
        {
            return await _searchService.FindEntityByEmailAsync<T>(email);
        }

        public async Task SignInUserAsync(TR user) 
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

        public async Task<TR> FindCurrentUserAsync() 
        {
            return await _searchService.FindCurrentUserAsync<TR>();
        }

        public async Task AddPictureProfileAsync(string imageFile, TR user) 
        {
            await _pictureService.AddPictureProfileAsync(imageFile, user);
        }

        public async Task VerifyDuplicateNameOrEmailAsync(string name, string email) 
        {
            await _verifyService.VerifyDuplicateNameOrEmailAsync(name, email);
        }

        public async Task<IEnumerable<T>> FindAllEntitiesAsync<T>() 
            where T : class, IEntity
        {
            return await _searchService.FindAllEntitiesAsync<T>();
        }

        public void SetSessionValue<T>(string key, T value)  
        {
            _httpHelper.SetSessionValue(key, value);
        }

        public T GetSessionValue<T>(string key) 
        {
            return _httpHelper.GetSessionValue<T>(key);
        }

        public void RemoveSessionValue(string key) 
        {
            _httpHelper.RemoveSessionValue(key);
        }

        public async Task CreateArtistAsync(RegisterUserViewModel artistVM) 
        {
            await _recordUserService.CreateArtistAsync(artistVM);
        }

        public async Task UpdateDescriptionAsync<T>(T user) 
            where T : class, IUser<T>
        {
            await _updateService.UpdateDescriptionAsync(user);
        }
    }
}
