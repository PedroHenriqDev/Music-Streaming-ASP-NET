using DataAccessLayer.Mappers;
using DataAccessLayer.Repositories;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction _transacion;
    private readonly DataMapper _mapper;

    private IPlaylistRepository? _playlistRepository;
    private IMusicRepository? _musicRepository;
    private IEntitiesAssociationRepository? _entitiesAssociationRepository;
    private IUserRepository? _userRepository;
    private IGenericRepository? _genericRepository;

    private UnitOfWork(NpgsqlConnection connection,
                       DataMapper mapper)
    {
        _connection = connection;
        _mapper = mapper;
    }

    public static async Task<UnitOfWork> CreateAsync(string sqlConnection, DataMapper dataMapper)
    {
        var connection = new NpgsqlConnection(sqlConnection);
        await connection.OpenAsync();
        return new UnitOfWork(connection, dataMapper);
    }

    public IPlaylistRepository PlaylistRepository => _playlistRepository ??= new PlaylistRepository(_connection, _mapper);
    public IMusicRepository MusicRepository => _musicRepository ??= new MusicRepository(_connection, _mapper);
    public IEntitiesAssociationRepository EntitiesAssociationRepository => _entitiesAssociationRepository ??= new EntitiesAssociationRepository(_connection, _mapper);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_connection, _mapper);
    public IGenericRepository GenericRepository => _genericRepository ??= new GenericRepository(_connection, _mapper);

    public void Dispose()
    {
        _transacion?.Dispose();
        _connection?.Dispose();
    }

    public async Task DisposeAsync()
    {
        _transacion?.DisposeAsync();
        _connection?.DisposeAsync();
    }
}
