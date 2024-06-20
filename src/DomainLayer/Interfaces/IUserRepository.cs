namespace DomainLayer.Interfaces;

public interface IUserRepository
{
    Task<T> GetUserByCredentialsAsync<T>(string email, string password) 
        where T : IUser<T>;

    Task<T> GetUserByNameAsync<T>(string name)
        where T : class, IUser<T>;

    Task<T> GetUserByIdAsync<T>(string id) 
        where T : class, IUser<T>;

    Task RecordUserAsync<T>(T user)
        where T : class, IUser<T>;

    Task UpdateUserProfilePictureAsync<T>(T user)
         where T : class, IUser<T>;
}
