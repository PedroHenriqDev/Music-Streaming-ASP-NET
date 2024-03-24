using MusicWeave.Data;
using MusicWeave.Exceptions;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.ViewModels;
using System.Xml.Linq;

namespace MusicWeave.Models.Services
{
    public class RegisterService
    {
        private readonly ILogger<RegisterService> _logger;
        private readonly ConnectionDb _connectionDb;
        private readonly VerifyService _verifyService;
        private readonly EncryptService _encryptService;

        public RegisterService(
            ILogger<RegisterService> logger,
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

        public async void CreateUserAsync(RegisterUserViewModel userVM) 
        {
            if (await _verifyService.HasNameInDbAsync<User>((User)userVM)) 
            {
                _logger.LogInformation("User creation attempt failed because the same name already exists in the database");
                throw new RegisterException("This name exist");
            }

            if(await _verifyService.HasEmailInDbAsync(userVM.Email)) 
            {
                _logger.LogInformation("User creation attempt failed because the same email already exists in the database");
                throw new RegisterException("This email exist");
            }

            userVM.Password = _encryptService.EncryptPasswordSHA512(userVM.Password);
        }

    }
}
