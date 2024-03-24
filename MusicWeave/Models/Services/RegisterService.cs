using MusicWeave.Data;
using MusicWeave.Exceptions;
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

        public RegisterService(
            ILogger<RegisterService> logger, 
            ConnectionDb connectionDb,
            VerifyService verifyService)
        {
            _logger = logger;
            _connectionDb = connectionDb;
            _verifyService = verifyService;
        }
    }
}
