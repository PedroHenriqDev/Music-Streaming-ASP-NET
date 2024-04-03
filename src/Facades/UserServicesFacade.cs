using Models.Entities;
using Models.Interfaces;
using Services;
using Utilities.Factories;
using ViewModels;

namespace Facades
{
    public class UserServicesFacade<T> where T : class, IUser<T>, new()
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
        private readonly ViewModelFactory _viewModelFactory;
        private readonly ModelFactory _modelFactory;

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
            UpdateService updateService,
            ViewModelFactory viewModelFactory,
            ModelFactory modelFactory)
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
            _viewModelFactory = viewModelFactory;
            _modelFactory = modelFactory;
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

        public void SetSessionValue<TR>(string key, TR value)  
        {
            _httpHelper.SetSessionValue(key, value);
        }

        public TR GetSessionValue<TR>(string key) 
        {
            return _httpHelper.GetSessionValue<TR>(key);
        }

        public void RemoveSessionValue(string key) 
        {
            _httpHelper.RemoveSessionValue(key);
        }

        public async Task CreateArtistAsync(RegisterUserViewModel artistVM) 
        {
            await _recordUserService.CreateArtistAsync(artistVM);
        }

        public async Task UpdateDescriptionAsync<TR>(TR user) 
            where TR : class, IEntityWithDescription<TR>
        {
            await _updateService.UpdateDescriptionAsync(user);
        }

        public DescriptionViewModel FactoryDescriptionViewModel<TR>(T entity) 
            where TR : class, IEntityWithDescription<TR>
        {
            return _viewModelFactory.FactoryDescriptionViewModel(entity);
        }

        public T FactoryUser(string id, string description) 
        {
            return _modelFactory.FactoryUser<T>(id, description);
        }
    }
}
