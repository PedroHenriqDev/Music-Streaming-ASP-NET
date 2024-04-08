using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;
using DomainLayer.Entities;
using DomainLayer.Exceptions;

namespace DataAccessLayer.Cloud
{
    public class ConnectionGoogleCloud
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public ConnectionGoogleCloud(IConfiguration configuration)
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

        public async Task<MusicData> DownloadMusicAsync(string musicId)
        {
            string objectName = $"{musicId}.zip";
            var downloadStream = new MemoryStream();

            await _storageClient.DownloadObjectAsync(_bucketName, objectName, downloadStream);
            downloadStream.Seek(0, SeekOrigin.Begin);

            using (var archive = new ZipArchive(downloadStream, ZipArchiveMode.Read))
            {
                var audioEntry = archive.GetEntry($"{musicId}_audio.mp3");
                var pictureEntry = archive.GetEntry($"{musicId}_picture.jpg");

                if (audioEntry == null || pictureEntry == null)
                    throw new MusicException("Music data not found in the archive.");

                using (var audioStream = audioEntry.Open())
                using (var pictureStream = pictureEntry.Open())
                {
                    var audioMemoryStream = new MemoryStream();
                    await audioStream.CopyToAsync(audioMemoryStream);
                    var pictureMemoryStream = new MemoryStream();
                    await pictureStream.CopyToAsync(pictureMemoryStream);

                    return new MusicData(musicId, audioMemoryStream.ToArray(), pictureMemoryStream.ToArray());
                }
            }
        }

        public async Task<IEnumerable<MusicData>> DownloadMusicsAsync(IEnumerable<string> musicIds)
        {
            var tasks = musicIds.Select(id => DownloadMusicAsync(id));
            return await Task.WhenAll(tasks);
        }
    }
}
