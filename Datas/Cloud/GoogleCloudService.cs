using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using MusicWeaveArtist.Cloud.CloudClasses;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

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
            using (var memoryStream = new MemoryStream(musicData.Data))
            {
                string objectName = $"{musicData.Id}.mp3"; 
                await _storageClient.UploadObjectAsync(_bucketName, objectName, null, memoryStream);
            }
        }
    }
}
    