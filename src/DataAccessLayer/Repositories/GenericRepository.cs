using Dapper;
using DataAccessLayer.Mappers;
using DataAccessLayer.Sanitization;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.Repositories;

public class GenericRepository : IGenericRepository
{
    private readonly NpgsqlConnection _connection;
    private readonly DataMapper _mapper;

    public GenericRepository(NpgsqlConnection connection,
                             DataMapper mapper)
    {
        _connection = connection;
        _mapper = mapper;
    }

    public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>()
        where T : class, IEntity
    {
        string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()}";
        return await _connection.QueryAsync<T>(sqlQuery);
    }

    public async Task<IEnumerable<T>> GetEntitiesByIdAsync<T>(string id)
        where T : class, IEntity
    {
        string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
        return await _connection.QueryAsync<T>(sqlQuery, new { id = id });
    }

    public async Task<IEnumerable<T>> GetEntitiesByIdsAsync<T>(IEnumerable<string> ids)
        where T : class, IEntity
    {
        var sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id IN ({FieldSanitization.JoinIds(ids)})";
        return await _connection.QueryAsync<T>(sqlQuery);
    }

    public async Task<IEnumerable<TTable>> GetEntitiesByFKAsync<TTable, TField>(string fkId)
        where TTable : class, IEntity where TField : class, IEntity
    {
        string tableName = TableNameSanitization.GetPluralTableName<TTable>();
        string fkField = FieldSanitization.ForeignKeyName<TField>();
        var sqlQuery = $"SELECT * FROM {tableName} WHERE {fkField} = @fkid";
        return await _connection.QueryAsync<TTable>(sqlQuery, new
        {
            fkId = fkId
        });
    }

    public async Task<IEnumerable<TTable>> GetEntitiesByForeignKeysAsync<TTable, TField>(IEnumerable<string> fkIds)
        where TTable : class, IEntity where TField : class, IEntity
    {
        string tableName = TableNameSanitization.GetPluralTableName<TTable>();
        string fieldFKName = FieldSanitization.ForeignKeyName<TField>();
        var sqlQuery = $"SELECT * FROM {tableName} WHERE {fieldFKName} IN ({FieldSanitization.JoinIds(fkIds)})";
        return await _connection.QueryAsync<TTable>(sqlQuery);
    }

    public async Task<T> GetEntityByIdAsync<T>(string id)
     where T : IEntity
    {
        string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
        return await _connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { id = id });
    }

    public async Task<T> GetEntityByEmailAsync<T>(string email)
        where T : IEntityWithEmail<T>
    {
        string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email";
        return await _connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email });
    }

    public async Task<T> GetEntityByNameAsync<T>(string name)
        where T : class, IEntityWithName<T>
    {
        string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
        return await _connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
    }

    public async Task UpdateDescriptionAsync<T>(T entity)
        where T : class, IEntityWithDescription<T>
    {
        string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET Description = @description WHERE Id = @id";
        await _connection.QueryAsync<T>(sqlQuery, new
        {
            description = entity.Description,
            id = entity.Id
        });
    }

    public async Task RemoveEntityByIdAsync<T>(string id)
       where T : IEntity
    {
        string sqlQuery = $"DELETE FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
        await _connection.QueryAsync(sqlQuery, new
        {
            id = id
        });
    }
}
