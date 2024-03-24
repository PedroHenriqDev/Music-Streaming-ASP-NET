using MusicWeave.Data;
using MusicWeave.Exceptions;
using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.ViewModels;
using System.Xml.Linq;
using System.Xml.Serialization;

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
            VerifyService verifyService)
        {
            _logger = logger;
            _connectionDb = connectionDb;
            _verifyService = verifyService;
        }

        private int RamdomId() 
        {
            Random random = new Random();
            return random.Next();
        }

        public async void CreateListenerAsync(RegisterListenerViewModel userVM) 
        {
            if (await _verifyService.HasNameInDbAsync<Listener>((Listener)userVM)) 
            {
                _logger.LogInformation("User creation attempt failed because the same name already exists in the database");
                throw new RegisterException("This name exist");
            }

            if(await _verifyService.HasEmailInDbAsync<Listener>((Listener)userVM))
            {
                _logger.LogInformation("User creation attempt failed because the same email already exists in the database");
                throw new RegisterException("This email exist");
            }

            await _connectionDb.CreateListenerAsync(new Listener(RamdomId(), userVM.Username, _encryptService.EncryptPasswordSHA512(userVM.Password), userVM.Email, userVM.PhoneNumber, userVM.Description, userVM.BirthDate));
        }
    }
}
