using Dapper;
using Npgsql;
using MusicWeave.Exceptions;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.Interfaces;

namespace MusicWeave.Datas
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

        public async Task<T> GetEntityByCredentialsAsync<T>(string email, string password)
            where T : IEntityWithEmail<T>
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("At the time of query the object was null");
                throw new ConnectionDbException("Objects used as a paramater is null");
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Email = @email AND Password = @password";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email, password = password });
            }
        }

        public User GetUserByEmail(string email)
        {
            if (email == null)
            {
                _logger.LogWarning("Email the time of query the object was null");
                throw new ConnectionDbException("Email used as a parameter is null");
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                string tableName = typeof(User).Name + "s";
                string sqlQuery = $"SELECT * FROM Users WHERE Email = @email";
                return connection.QueryFirstOrDefault<User>(sqlQuery, new { email = email });
            }
        }

        public async Task<T> GetEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Email the time of query the object was null");
                throw new ConnectionDbException("Email used as a parameter is null");
            }

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
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("At the time of query the object was null");
                throw new ConnectionDbException("Object used as a parameter is null");
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task CreateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("At the time of query the object was null");
                throw new ConnectionDbException("Object used as a parameter is null");
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = $@"INSERT INTO Users (Id, Email, Name, Password, Description, BirthDate, PictureProfile, UserType, PhoneNumber, DateCreation) 
                                     VALUES (@id, @email, @name, @password, @description, @birthDate, @pictureProfile, @userType, @phoneNumber, @dateCreation)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name,
                    password = user.Password,
                    description = user.Description,
                    birthDate = user.BirthDate,
                    pictureProfile = user.PictureProfile,
                    userType = user.UserType,
                    phoneNumber = user.PhoneNumber,
                    dateCreation = user.DateCreation,
                });
            }
        }

        public async Task AddUserProfilePictureAsync(User user) 
        {
            if(user == null) 
            {
                _logger.LogError("An error ocurred while added picture profile.");
                throw new ConnectionDbException("An error ocurred, because of null user reference!");
            }

            using(NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString())) 
            {
                await connection.OpenAsync();
                string sqlQuery = $"UPDATE Users SET PictureProfile = @pictureProfile WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new { pictureProfile = user.PictureProfile, id = user.Id });
            }
        }
    }
}

