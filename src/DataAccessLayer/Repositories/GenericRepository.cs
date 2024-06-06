using Dapper;
using DataAccessLayer.Sanitization;
using DataAccessLayer.Sql;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.Repositories
{
    public class GenericRepository
    {
        private readonly ConnectionDb _connectionDb;

        public GenericRepository(ConnectionDb connectionDb)
        {
            _connectionDb = connectionDb;
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>()
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()}";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByIdAsync<T>(string id)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                return await connection.QueryAsync<T>(sqlQuery, new { id = id });
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByIdsAsync<T>(IEnumerable<string> ids)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                var sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id IN ({FieldSanitization.JoinIds(ids)})";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<IEnumerable<TTable>> GetEntitiesByFKAsync<TTable, TField>(string fkId)
            where TTable : class, IEntity where TField : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = TableNameSanitization.GetPluralTableName<TTable>();
                string fkField = FieldSanitization.ForeignKeyName<TField>();
                var sqlQuery = $"SELECT * FROM {tableName} WHERE {fkField} = @fkid";
                return await connection.QueryAsync<TTable>(sqlQuery, new
                {
                    fkId = fkId
                });
            }
        }

        public async Task<IEnumerable<TTable>> GetEntitiesByForeignKeysAsync<TTable, TField>(IEnumerable<string> fkIds)
            where TTable : class, IEntity where TField : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = TableNameSanitization.GetPluralTableName<TTable>();
                string fieldFKName = FieldSanitization.ForeignKeyName<TField>();
                var sqlQuery = $"SELECT * FROM {tableName} WHERE {fieldFKName} IN ({FieldSanitization.JoinIds(fkIds)})";
                return await connection.QueryAsync<TTable>(sqlQuery);
            }
        }

        public async Task<T> GetEntityByCredentialsAsync<T>(string email, string password)
            where T : IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email AND Password = @password";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new
                {
                    email = email,
                    password = password
                });
            }
        }

        public async Task<T> GetEntityByIdAsync<T>(string id)
         where T : IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { id = id });
            }
        }

        public async Task<T> GetEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email });
            }
        }

        public async Task<T> GetEntityByNameAsync<T>(string name)
            where T : class, IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task UpdateDescriptionAsync<T>(T entity)
            where T : class, IEntityWithDescription<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET Description = @description WHERE Id = @id";
                await connection.QueryAsync<T>(sqlQuery, new
                {
                    description = entity.Description,
                    id = entity.Id
                });
            }
        }

        public async Task RemoveEntityByIdAsync<T>(string id)
           where T : IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"DELETE FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new
                {
                    id = id
                });
            }
        }
    }
}

