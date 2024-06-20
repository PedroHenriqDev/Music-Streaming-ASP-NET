using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces;

public interface ICloudStorageService
{
    Task UploadMusicAsync(MusicData musicData);
    
    Task<MusicData> DownloadMusicAsync(string musicId);

    Task<IEnumerable<MusicData>> DownloadMusicsAsync(IEnumerable<string> musicIds);
}
