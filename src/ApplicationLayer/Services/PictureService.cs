using DomainLayer.Interfaces;
using UtilitiesLayer.Extensions;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Services
{
    public class PictureService
    {
        private readonly UpdateService _updateService;

        public PictureService(UpdateService updateService)
        {
            _updateService = updateService;
        }

        public async Task AddPictureProfileAsync<T>(string imageFile, T user) where T : class, IUser<T>
        {
            const int MAX_IMAGE_SIZE_BYTES = 5 * 1024 * 1024;

            if (string.IsNullOrWhiteSpace(imageFile) || user is null)
            {
                throw new ArgumentNullException("Object null exception.");
            }

            if (imageFile.Length > MAX_IMAGE_SIZE_BYTES)
            {
                throw new OverflowException("Image size exceeds the maximum allowed size.");
            }

            string base64WithoutPrefix = imageFile.Replace("data:image/jpeg;base64,", "");
            user.PictureProfile = Convert.FromBase64String(base64WithoutPrefix);
            await _updateService.UpdateUserPictureProfileAsync(user);
        }

        public async Task<string> SavePictureProfileAsync(byte[] pictureData, string webRootPath)
        {
            string profilePictureUrl = string.Empty;
            string profilePictureDiretory = CreateProfilePictureDirectory(webRootPath);
            string fileExtension = GetImageExtension(pictureData);

            string fileName = EncryptHelper.GetFileHash(pictureData) + fileExtension;
            string filePath = Path.Combine(profilePictureDiretory, fileName);

            if (!File.Exists(filePath))
            {
                await SaveFileAsync(filePath, pictureData);
            }

            return profilePictureUrl = $"/Profile-Pictures/{fileName}";
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
            if (pictureData.IsWebPImage())
            {
                return ".webp";
            }
            else if (pictureData.IsPngImage())
            {
                return ".png";
            }
            else if (pictureData.IsJpegImage())
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
    }
}
