using MusicWeave.Data;
using MusicWeave.Models.ViewModels;

namespace MusicWeave.Models.Services
{
    public class LoginService
    {
        private readonly EncryptService _encryptService;
        private readonly ConnectionDb _connectionDb;
            
        public LoginService(
            EncryptService encryptService,
            ConnectionDb connectionDb) 
        {
            _encryptService = encryptService;
            _connectionDb = connectionDb;
        }
    }
}
