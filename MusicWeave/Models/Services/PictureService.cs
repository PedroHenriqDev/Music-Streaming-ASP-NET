using MusicWeave.Datas;

namespace MusicWeave.Models.Services
{
    public class PictureService
    {

        private readonly ConnectionDb _connectionDb;
        private readonly EncryptService _encryptService;

        public PictureService(ConnectionDb connectionDb, EncryptService encryptService)
        {
            _connectionDb = connectionDb;
            _encryptService = encryptService;
        }

        public async Task<string> SavePictureProfileAsync(byte[] pictureData, string webRootPath)
        {
            string profilePictureUrl = string.Empty;
            string profilePictureDiretory = CreateProfilePictureDirectory(webRootPath);
            string fileExtension = GetImageExtension(pictureData);

            string fileName = _encryptService.GetFileHash(pictureData) + fileExtension;
            string filePath = Path.Combine(profilePictureDiretory, fileName);

            if (!File.Exists(filePath))
            {
                await SaveFileAsync(filePath, pictureData);
            }

            return profilePictureUrl = $"/profile-pictures/{fileName}";
        }

        private string CreateProfilePictureDirectory(string webRootPath)
        {
            string profilePictureDiretory = Path.Combine(webRootPath, "Profile-Pictures");

            if (!Directory.Exists(profilePictureDiretory))
            {
                Directory.CreateDirectory(profilePictureDiretory);
            }

            return profilePictureDiretory;
        }

        private string GetImageExtension(byte[] pictureData)
        {
            if (IsWebPImage(pictureData))
            {
                return ".webp";
            }
            else if (IsPngImage(pictureData))
            {
                return ".png";
            }
            else if (IsJpegImage(pictureData))
            {
                return ".jpeg";
            }
            else
            {
                return ".jpg";
            }
        }

        private async Task SaveFileAsync(string filePath, byte[] pictureData)
        {
            await File.WriteAllBytesAsync(filePath, pictureData);
        }

        private bool IsWebPImage(byte[] imageData)
        {
            return imageData.Length > 12 &&
                   imageData[0] == 0x52 &&
                   imageData[1] == 0x49 &&
                   imageData[2] == 0x46 &&
                   imageData[3] == 0x46 &&
                   imageData[8] == 0x57 &&
                   imageData[9] == 0x45 &&
                   imageData[10] == 0x42 &&
                   imageData[11] == 0x50;
        }

        private bool IsJpegImage(byte[] imageData)
        {
            return imageData.Length > 3 &&
                   imageData[0] == 0xFF &&
                   imageData[1] == 0xD8 &&
                   imageData[2] == 0xFF;
        }

        private bool IsPngImage(byte[] imageData)
        {
            return imageData.Length > 4 &&
                   imageData[0] == 0x89 &&
                   imageData[1] == 0x50 &&
                   imageData[2] == 0x4E &&
                   imageData[3] == 0x47;
        }
    }
}
