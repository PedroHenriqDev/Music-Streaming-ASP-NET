using DomainLayer.Interfaces;

namespace ApplicationLayer.Interfaces;

public interface IUpdateService
{
    Task UpdateDescriptionAsync<T>(T entity)
     where T : class, IEntityWithDescription<T>;

    Task UpdateUserPictureProfileAsync<T>(T user)
        where T : class, IUser<T>;
}
