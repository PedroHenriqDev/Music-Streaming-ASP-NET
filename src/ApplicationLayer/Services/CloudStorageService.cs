using ApplicationLayer.Interfaces;
using DataAccessLayer.Cloud;
using DomainLayer.Entities;
using DomainLayer.Exceptions;

namespace ApplicationLayer.Services;

public class CloudStorageService : ICloudStorageService
{

    private readonly ConnectionGoogleCloud _connectionCloud;

    public CloudStorageService(ConnectionGoogleCloud connectionCloud) 
    {
        _connectionCloud = connectionCloud;
    }

    public async Task UploadMusicAsync(MusicData musicData) 
    {
        try 
        {
            if(musicData.Audio is null || musicData.Picture is null) 
            {
                throw new MusicException("Music must have audio and image!");
            }
            
            await _connectionCloud.UploadMusicAsync(musicData);
        }
        catch (Exception)
        {
            throw new MusicException("Error in upload music!");
        }
    }

    public async Task<MusicData> DownloadMusicAsync(string musicId) 
    {
        return await _connectionCloud.DownloadMusicAsync(musicId);
    }

    public async Task<IEnumerable<MusicData>> DownloadMusicsAsync(IEnumerable<string> musicIds)
    {
       return await _connectionCloud.DownloadMusicsAsync(musicIds);
    }
}
