using MusicWeave.Datas;
using MusicWeave.Exceptions;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.ViewModels;

namespace MusicWeave.Models.Services
{
    public class RegisterUserService
    {
        private readonly ILogger<RegisterUserService> _logger;
        private readonly ConnectionDb _connectionDb;
        private readonly VerifyService _verifyService;
        private readonly EncryptService _encryptService;

        public RegisterUserService(
            ILogger<RegisterUserService> logger,
            ConnectionDb connectionDb,
            VerifyService verifyService,
            EncryptService encryptService)
        {
            _logger = logger;
            _connectionDb = connectionDb;
            _verifyService = verifyService;
            _encryptService = encryptService;
        }

        private int RamdomId() 
        {
            Random random = new Random();
            return random.Next();
        }

        public async Task CreateListenerAsync(RegisterListenerViewModel listenerVM) 
        {
            Listener listener = new Listener(
                RamdomId(),
                listenerVM.Name,
                _encryptService.EncryptPasswordSHA512(listenerVM.Password),
                listenerVM.Email,
                listenerVM.PhoneNumber,
                typeof(Listener).Name,
                listenerVM.BirthDate,
                DateTime.Now);

            if (await _verifyService.HasNameInDbAsync<User>(listener.Name)) 
            {
                _logger.LogInformation("User creation attempt failed because the same name already exists in the database");
                throw new RegisterException("This name exist");
            }

            if(await _verifyService.HasNameInDbAsync<User>(listener.Name) || await _verifyService.HasNameInDbAsync<User>(listener.Name))
            {
                _logger.LogInformation("User creation attempt failed because the same email already exists in the database");
                throw new RegisterException("This email exist");
            }

            await _connectionDb.CreateUserAsync(listener);
        }

        public async Task CreateArtistAsync(RegisterArtistViewModel artistVM) 
        {
            Artist artist = new Artist(
                RamdomId(),
                artistVM.Name,
                _encryptService.EncryptPasswordSHA512(artistVM.Password),
                artistVM.Email,
                artistVM.PhoneNumber,
                typeof(Artist).Name,
                artistVM.BirthDate,
                DateTime.Now);

            if(await _verifyService.HasNameInDbAsync<User>(artist.Name)) 
            {
                _logger.LogInformation("User creation attempt failed because the same name already exists in the database");
                throw new RegisterException("Existing name.");
            }

            if(await _verifyService.HasEmailInDbAsync<User>(artist.Email) || await _verifyService.HasEmailInDbAsync<User>(artist.Email))
            {
                _logger.LogInformation("User creation attempt failed because the same email already exists in the database");
                throw new RegisterException("Existing email.");
            }

            await _connectionDb.CreateUserAsync(artist);
        }
    }
}
