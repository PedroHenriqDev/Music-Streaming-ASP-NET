using Dapper;
using Npgsql;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using DomainLayer.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Sanitization;

namespace DataAccessLayer.Sql
{
    public class ConnectionDb
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConnectionDb> _logger;

        public ConnectionDb(
            IConfiguration configuration,
            ILogger<ConnectionDb> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>()
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()}";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByIdAsync<T>(string id)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                return await connection.QueryAsync<T>(sqlQuery, new { id = id });
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByIdsAsync<T>(IEnumerable<string> ids)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                var sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id IN ({FieldSanitization.JoinIds(ids)})";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByForeignKeysAsync<T, TR>(IEnumerable<string> fkIds)
            where T : class, IEntity where TR : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = TableNameSanitization.GetPluralTableName<T>();
                string fieldFKName = FieldSanitization.ForeignKeyName(typeof(TR).Name);
                var sqlQuery = $"SELECT * FROM {tableName} WHERE {fieldFKName} IN ({FieldSanitization.JoinIds(fkIds)})";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<T> GetEntityByCredentialsAsync<T>(string email, string password)
            where T : IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email AND Password = @password";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email, password = password });
            }
        }

        public T GetUserByName<T>(string name)
            where T : IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
                return connection.QueryFirst<T>(sqlQuery, new { name = name });
            }
        }

        public async Task<T> GetEntityByIdAsync<T>(string id)
            where T : IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { id = id });
            }
        }

        public async Task<T> GetUserByNameAsync<T>(string name)
            where T : IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task<T> GetEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email });
            }
        }

        public async Task<T> GetEntityByNameAsync<T>(string name)
            where T : class, IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task<IEnumerable<UserGenre<T>>> GetUserGenresAsync<T>(string id)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetAssociationTableGenre<T>()} WHERE Id = @id";
                return await connection.QueryAsync<UserGenre<T>>(sqlQuery, new { id = id });
            }
        }

        public async Task<IEnumerable<Music>> GetMusicsByGenreIdsAsync(IEnumerable<string> genreIds)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = @$"
                                   SELECT
                                       m.Id,
                                       m.Name,
                                       m.ArtistId,
                                       m.GenreId,
                                       m.Date,
                                       m.DateCreation,
                                       a.Id AS ArtistId,
                                       a.Name AS Name
                                   FROM
                                       Musics m
                                   INNER JOIN
                                       Artists a ON m.ArtistId = a.Id
                                   WHERE
                                      m.GenreId = ANY(@genreIds)";

                var result = await connection.QueryAsync<Music, Artist, Music>(
                        sqlQuery,
                        (music, artist) =>
                        {
                            if (artist != null)
                                music.Artist = artist;
                            return music;
                        },
                        splitOn: "ArtistId",
                        param: new { genreIds });

                return result;
            }
        }

        public async Task RecordUserAsync<T>(T user)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = $@"INSERT INTO {TableNameSanitization.GetPluralTableName<T>()} (Id, Email, Name, Password, Description, BirthDate, PictureProfile, PhoneNumber, DateCreation) 
                                     VALUES (@id, @email, @name, @password, @description, @birthDate, @pictureProfile, @phoneNumber, @dateCreation)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name,
                    password = user.Password,
                    description = user.Description,
                    birthDate = user.BirthDate,
                    pictureProfile = user.PictureProfile,
                    phoneNumber = user.PhoneNumber,
                    dateCreation = user.DateCreation,
                });
            }
        }

        public async Task RecordUserGenresAsync<T>(List<UserGenre<T>> userGenres)
             where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();

                string tableName = TableNameSanitization.GetAssociationTableGenre<T>();
                string sqlQuery = $"INSERT INTO {tableName} (Id, GenreId) VALUES (@Id, @GenreId)";

                foreach (var userGenre in userGenres)
                {
                    await connection.ExecuteAsync(sqlQuery, new { Id = userGenre.Id, GenreId = userGenre.GenreId });
                }
            }
        }

        public async Task RecordUserAndUserGenresAsync<T>(T user, List<UserGenre<T>> userGenres)
            where T : class, IUser<T>
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        await RecordUserAsync(user);
                        await RecordUserGenresAsync(userGenres);
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new RecordAssociationException(ex.Message);
                    }
                }
            }
        }

        public async Task RecordMusicAsync(Music music)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = @"INSERT INTO Musics (Id, Name, ArtistId, GenreId, Date, DateCreation) 
                                    VALUES(@id, @name, @artistId, @genreId, @date, @dateCreation)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = music.Id,
                    name = music.Name,
                    artistId = music.ArtistId,
                    genreId = music.GenreId,
                    date = music.Date,
                    dateCreation = music.DateCreation,
                });
            }
        }

        public async Task UpdateUserProfilePictureAsync<T>(T user)
            where T : IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET PictureProfile = @pictureProfile WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new { pictureProfile = user.PictureProfile, id = user.Id });
            }
        }

        public async Task UpdateDescriptionAsync<T>(T entity)
            where T : class, IEntityWithDescription<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET Description = @description WHERE Id = @id";
                await connection.QueryAsync<T>(sqlQuery, new { description = entity.Description, id = entity.Id });
            }
        }

        public async Task DeleteEntityByIdAsync<T>(string id)
            where T : IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"DELETE FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new { id = id });
            }
        }
    }
}

