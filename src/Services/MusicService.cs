using Datas.Cloud;
using MusicWeaveArtist.Cloud.CloudClasses;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Exceptions;
using ViewModels;
using Utilities.Helpers;
using Datas.Sql;
using Models.ConcreteClasses;
using Microsoft.Extensions.Logging;
using System.Management;

namespace Services
{
    public class MusicService
    {
        private readonly GoogleCloudService _googleCloudService;
        private readonly ConnectionDb _connectionDb;
        private readonly ByteConvertHelper _byteHelper;
        private readonly SearchService _searchService;
        private readonly ILogger<MusicService> _logger;

        public MusicService(
            GoogleCloudService googleCloudService,
            ByteConvertHelper byteHelper,
            ConnectionDb connectionDb,
            SearchService searchService, ILogger<MusicService> logger)
        {
            _googleCloudService = googleCloudService;
            _byteHelper = byteHelper;
            _connectionDb = connectionDb;
            _searchService = searchService;
            _logger = logger;
        }

        public async Task AddMusicAsync(AddMusicViewModel musicVM)
        {
            string id = Guid.NewGuid().ToString();
            try
            {
                await _connectionDb.RecordMusicAsync(await ParseMusicAsync(musicVM, id));
                await _googleCloudService.UploadMusicAsync(await ParseMusicDataAsync(musicVM, id));
            }
            catch(Exception ex) 
            {
                _logger.LogError("An error ocurred while recording music data!");
                await _connectionDb.DeleteEntityByIdAsync<Music>(id);
                throw;
            }
        }

        public async Task<Music> ParseMusicAsync(AddMusicViewModel musicVM, string id)
        {
            Artist artist = await _searchService.FindCurrentUserAsync<Artist>();
            if (artist == null)
            {
                _logger.LogWarning("Error occurred due to a null user reference");
                throw new ArgumentNullException("Error occurred when searching for the user");
            }
            return new Music(id, musicVM.Name, artist.Id, musicVM.GenreId, musicVM.Date, DateTime.Now);
        }

        public async Task<MusicData> ParseMusicDataAsync(AddMusicViewModel musicVM, string Id)
        {
            if (musicVM.Audio == null && musicVM.Picture == null)
            {
                throw new MusicException("Music must have audio and image");
            }
            byte[] audioBytes = await _byteHelper.ConvertIFormFileInByte(musicVM.Audio);
            byte[] pictureBytes = await _byteHelper.ConvertIFormFileInByte(musicVM.Picture);
            return new MusicData(Id, audioBytes, pictureBytes);
        }
    }
}
