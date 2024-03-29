using Datas.Cloud;
using MusicWeaveArtist.Cloud.CloudClasses;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Exceptions;
using ViewModels;

namespace Services
{
    public class MusicService
    {
        private readonly GoogleCloudService _googleCloudService;

        public MusicService(GoogleCloudService googleCloudService)
        {
            _googleCloudService = googleCloudService;
        }

        public async Task AddMusicAsync(AddMusicViewModel musicVM)
        {
            await _googleCloudService.UploadMusicAsync(await ParseMusicDataAsync(musicVM, Guid.NewGuid().ToString()));
        }

        public async Task<MusicData> ParseMusicDataAsync(AddMusicViewModel musicVM, string Id)
        {
            if (musicVM.MusicData == null && musicVM.MusicData.Length > 0)
            {
                throw new MusicException("Error in record music data!");
            }
            else
            {
                byte[] audioBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await musicVM.MusicData.CopyToAsync(memoryStream);
                    audioBytes = memoryStream.ToArray();
                }

                return new MusicData
                {
                    Id = Id,
                    Data = audioBytes
                };
            }
        }
    }
}
