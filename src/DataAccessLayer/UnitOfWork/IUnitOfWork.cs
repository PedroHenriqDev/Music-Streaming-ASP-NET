using DomainLayer.Interfaces;

namespace DataAccessLayer.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IPlaylistRepository PlaylistRepository { get; }
    IMusicRepository MusicRepository { get; }
    IEntitiesAssociationRepository EntitiesAssociationRepository { get; }
    IUserRepository UserRepository { get; }
    IGenericRepository GenericRepository { get; }

    void Dispose();
    Task DisposeAsync();
}
