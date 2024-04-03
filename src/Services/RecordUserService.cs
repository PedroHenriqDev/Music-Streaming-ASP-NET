using Datas.Sql;
using Microsoft.Extensions.Logging;
using Exceptions;
using ViewModels;
using Models.Entities;
using Models.Queries;

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

        public async Task CreateUserGenresAsync(string userId, List<string> genreIds)
        {
            if (string.IsNullOrWhiteSpace(userId) || genreIds.Any())
            {
                _logger.LogInformation("An error ocurred while record User Genre");
                throw new ArgumentNullException("Error in record User Genre.");
            }

            await _connectionDb.RecordEntityAssociationsAsync<UserGenre>(userId, genreIds);
        }

        public async Task<EntityQuery<Listener>> CreateListenerAsync(RegisterUserViewModel listenerVM)
        {
            Listener listener = new Listener(Guid.NewGuid().ToString(), listenerVM.Name, _encryptService.EncryptPasswordSHA512(listenerVM.Password), listenerVM.Email, listenerVM.PhoneNumber, listenerVM.BirthDate, DateTime.Now);
            try
            {
                await _connectionDb.RecordListenerAsync(listener);
                await _connectionDb.RecordEntityAssociationsAsync<UserGenre>(listener.Id, listenerVM.SelectedGenreIds);
                return new EntityQuery<Listener>(true, "Listener created successfully", listener, DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError("Brutal error in method CreateListenerAsync");
                throw new RecordException<EntityQuery<Listener>>($"This error occurred while registration was happening, {ex.Message}", new EntityQuery<Listener>(false, "Unable to create a listener", listener, DateTime.Now));
            }
        }

        public async Task<EntityQuery<Artist>> CreateArtistAsync(RegisterUserViewModel artistVM)
        {
            Artist artist = new Artist(Guid.NewGuid().ToString(), artistVM.Name, _encryptService.EncryptPasswordSHA512(artistVM.Password), artistVM.Email, artistVM.PhoneNumber, artistVM.BirthDate, DateTime.Now);
            try
            {
                await _connectionDb.RecordArtistAsync(artist);
                await _connectionDb.RecordEntityAssociationsAsync<UserGenre>(artist.Id, artistVM.SelectedGenreIds);
                return new EntityQuery<Artist>(true, "Artist created successfully", artist, DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError("Brutal error in method CreateArtistAsync");
                throw new RecordException<EntityQuery<Artist>>($"This error occurred while registration was happening, {ex.Message}", new EntityQuery<Artist>(false, "Unable to create a artist", artist, DateTime.Now));
            }

        }
    }
}
