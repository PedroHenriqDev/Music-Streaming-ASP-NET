using Dapper;
using Npgsql;
using Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Interfaces;
using Models.ConcreteClasses;

namespace Datas.Sql
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
            using(NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString())) 
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName}";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<T> GetEntityByCredentialsAsync<T>(string email, string password)
            where T : IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Email = @email AND Password = @password";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email, password = password });
            }
        }

        public T GetUserByEmail<T>(string email)
            where T : IEntityWithEmail<T> 
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Email = @email";
                return connection.QueryFirst<T>(sqlQuery, new { email = email });
            }
        }

        public async Task<T> GetEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Email = @email";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email });
            }
        }

        public async Task<T> GetEntityByNameAsync<T>(string name)
            where T : class, IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task RecordListenerAsync(Listener listener)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = $@"INSERT INTO Listeners (Id, Email, Name, Password, Description, BirthDate, PictureProfile, PhoneNumber, DateCreation) 
                                     VALUES (@id, @email, @name, @password, @description, @birthDate, @pictureProfile, @phoneNumber, @dateCreation)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = listener.Id,
                    email = listener.Email,
                    name = listener.Name,
                    password = listener.Password,
                    description = listener.Description,
                    birthDate = listener.BirthDate,
                    pictureProfile = listener.PictureProfile,
                    phoneNumber = listener.PhoneNumber,
                    dateCreation = listener.DateCreation,
                });
            }
        }

        public async Task RecordUserGenresAsync(string userId, List<string> genreIds)
        {
            using(NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = "INSERT INTO UserGenres (UserId, GenreId) VALUES";

                for(int i = 0;  i < genreIds.Count; i++) 
                {
                    sqlQuery += $"('{userId}'), '{genreIds[i]}";

                    if(i < genreIds.Count() - 1) 
                    {
                        sqlQuery += ",";
                    }
                }

                using(NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task RecordArtistAsync(Artist artist)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = $@"INSERT INTO Artists (Id, Email, Name, Password, Description, BirthDate, PictureProfile, PhoneNumber, DateCreation) 
                                     VALUES (@id, @email, @name, @password, @description, @birthDate, @pictureProfile, @phoneNumber, @dateCreation)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = artist.Id,
                    email = artist.Email,
                    name = artist.Name,
                    password = artist.Password,
                    description = artist.Description,
                    birthDate = artist.BirthDate,
                    pictureProfile = artist.PictureProfile,
                    phoneNumber = artist.PhoneNumber,
                    dateCreation = artist.DateCreation,
                });
            }
        }

        public async Task AddUserProfilePictureAsync<T>(T user) where T : IUser<T> 
        {
            using(NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString())) 
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"UPDATE {tableName} SET PictureProfile = @pictureProfile WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new { pictureProfile = user.PictureProfile, id = user.Id });
            }
        }
    }
}

