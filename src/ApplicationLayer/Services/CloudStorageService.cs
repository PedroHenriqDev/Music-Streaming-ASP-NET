using ApplicationLayer.Factories;
using DataAccessLayer.Cloud;
using DomainLayer.Entities;
using DomainLayer.Exceptions;

namespace ApplicationLayer.Services
{
    public class CloudStorageService
    {

        private readonly ConnectionGoogleCloud _connectionCloud;
        private readonly ModelFactory _modelFactory;

        public CloudStorageService(ConnectionGoogleCloud connectionCloud, ModelFactory modelFactory) 
        {
            _connectionCloud = connectionCloud;
            _modelFactory = modelFactory;
        }

        public async Task UploadMusicAsync(MusicData musicData) 
        {
            try 
            {
                if(musicData.Audio == null || musicData.Picture == null) 
                {
                    throw new MusicException("Music must have audio and image");
                }

                await _connectionCloud.UploadMusicAsync(musicData);
            }
            catch (Exception)
            {
                throw new MusicException("Error in upload music!");
            }
        }
    }
}
