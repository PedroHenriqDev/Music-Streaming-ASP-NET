using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Dapper;
using MusicWeave.Exceptions;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.Interfaces;
using MusicWeave.Models.ViewModels;

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

        public async Task<T> GetUserByCredentialsAsync<T>(string email, string password)
            where T : IEntityWithEmail<T>
        {
            if (email == null || password == null)
            {
                _logger.LogWarning("At the time of query the object was null");
                throw new ConnectionDbException("Objects used as a paramater is null");
            }

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Email = @email AND Password = @password";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email, password = password });
            }
        }

        public async Task<T> GetUserByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            if (email == null)
            {
                _logger.LogWarning("Email the time of query the object was null");
                throw new ConnectionDbException("Email used as a parameter is null");
            }

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
            if (name == null)
            {
                _logger.LogWarning("At the time of query the object was null");
                throw new ConnectionDbException("Object used as a parameter is null");
            }

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
    }
}

