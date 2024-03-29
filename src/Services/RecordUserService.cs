using Datas.Sql;
using Microsoft.Extensions.Logging;
using Exceptions;
using Models.ConcreteClasses;
using ViewModels;

namespace Services
{
    public class RecordUserService
    {
        private readonly ILogger<RecordUserService> _logger;
        private readonly ConnectionDb _connectionDb;
        private readonly VerifyService _verifyService;
        private readonly EncryptService _encryptService;

        public RecordUserService(
            ILogger<RecordUserService> logger,
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

        public async Task CreateUserGenresAsync(string userId, List<string> genreIds)
        {
            if (string.IsNullOrWhiteSpace(userId) || genreIds.Any())
            {
                _logger.LogInformation("An error ocurred while record User Genre");
                throw new RecordException("Error in record User Genre.");
            }

            await _connectionDb.RecordEntityAssociationsAsync<UserGenre>(userId, genreIds);
        }

        public async Task CreateListenerAsync(RegisterListenerViewModel listenerVM)
        {
            Listener listener = new Listener(
                Guid.NewGuid().ToString(),
                listenerVM.Name,
                _encryptService.EncryptPasswordSHA512(listenerVM.Password),
                listenerVM.Email,
                listenerVM.PhoneNumber,
                listenerVM.BirthDate,
                DateTime.Now);

            if (await _verifyService.HasNameInDbAsync<Listener>(listener.Name) || await _verifyService.HasNameInDbAsync<Artist>(listener.Name))
            {
                _logger.LogInformation("User creation attempt failed because the same name already exists in the database");
                throw new RecordException("This name exist");
            }

            if (await _verifyService.HasEmailInDbAsync<Artist>(listener.Email) || await _verifyService.HasEmailInDbAsync<Listener>(listener.Email))
            {
                _logger.LogInformation("User creation attempt failed because the same email already exists in the database");
                throw new RecordException("Existing email.");
            }

            await _connectionDb.RecordListenerAsync(listener);
        }

        public async Task CreateArtistAsync(RegisterUserViewModel artistVM)
        {
            Artist artist = new Artist(
                Guid.NewGuid().ToString(),
                artistVM.Name,
                _encryptService.EncryptPasswordSHA512(artistVM.Password),
                artistVM.Email,
                artistVM.PhoneNumber,
                artistVM.BirthDate,
                DateTime.Now);

            await _connectionDb.RecordArtistAsync(artist);
            await _connectionDb.RecordEntityAssociationsAsync<UserGenre>(artist.Id, artistVM.GenreIds);
        }
    }
}
