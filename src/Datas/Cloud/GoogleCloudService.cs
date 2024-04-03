using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using Models.Entities;

namespace Datas.Cloud
{
    public class GoogleCloudService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public GoogleCloudService(IConfiguration configuration)
        {
            var googleCloudCredentialPath = configuration["GoogleCloudCredentialPath"];
            _bucketName = configuration["GoogleCloudStorageBucketName"];

            GoogleCredential credential;
            using (var jsonStream = new FileStream(googleCloudCredentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(jsonStream);
            }

            _storageClient = StorageClient.Create(credential);
        }

        public async Task UploadMusicAsync(MusicData musicData)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var audioEntry = archive.CreateEntry($"{musicData.Id}_audio.mp3");
                    using (var audioStream = audioEntry.Open())
                    {
                        await audioStream.WriteAsync(musicData.Audio, 0, musicData.Audio.Length);
                    }

                    var pictureEntry = archive.CreateEntry($"{musicData.Id}_picture.jpg");
                    using (var pictureStream = pictureEntry.Open())
                    {
                        await pictureStream.WriteAsync(musicData.Picture, 0, musicData.Picture.Length);
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                string objectName = $"{musicData.Id}.zip";
                await _storageClient.UploadObjectAsync(_bucketName, objectName, null, memoryStream);
            }
        }
    }
}
