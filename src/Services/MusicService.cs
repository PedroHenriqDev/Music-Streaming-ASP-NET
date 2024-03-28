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
        private readonly GoogleCloudService _firestoreService;

        public MusicService(GoogleCloudService firestoreService)
        {
            _firestoreService = firestoreService;
        }

        public async Task AddMusicAsync(MusicViewModel musicVM)
        {
            await _firestoreService.UploadMusicAsync(await ParseMusicDataAsync(musicVM, Guid.NewGuid().ToString()));
        }

        public async Task<MusicData> ParseMusicDataAsync(MusicViewModel musicVM, string Id)
        {
            if (musicVM.File == null && musicVM.File.Length > 0)
            {
                throw new MusicException("Error in record music data!");
            }
            else
            {
                byte[] audioBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await musicVM.File.CopyToAsync(memoryStream);
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
