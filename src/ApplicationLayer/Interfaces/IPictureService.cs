using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface IPictureService
{
    Task AddPictureProfileAsync<T>(string imageFile, T user)
        where T : class, IUser<T>;


    Task<string> SavePictureProfileAsync(byte[] pictureData, string webRootPath);
}
