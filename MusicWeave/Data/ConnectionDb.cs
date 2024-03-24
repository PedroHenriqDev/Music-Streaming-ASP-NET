using System.Data.SqlClient;
using Dapper;
using MusicWeave.Exceptions;
using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.Interfaces;

namespace MusicWeave.Data
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
        
        public async Task<T> GetEntityByNameAsync<T>(T entity) 
            where T : class, IEntityWithName<T>
        {
            if(entity == null) 
            {
                _logger.LogWarning("At the time of query the object was null");
                throw new ConnectionDbException("Object used as a parameter is null");
            }

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = entity.Name });
            }
        }

        public async Task<IUser<T>> GetUserByEmailAsync<T>(T user) 
            where T : class, IUser<T> 
        {
            if(user == null) 
            {
                _logger.LogWarning("At the time of query the user was null");
                throw new ConnectionDbException("User used as a paramater is null");
            }

            using(SqlConnection connection = new SqlConnection(GetConnectionString())) 
            {
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Email = @email";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new {email = user.Email });
            }
        }

        public async Task CreateListenerAsync(Listener listener) 
        {
            if(listener == null) 
            {
                _logger.LogWarning("At the time of query the user was null");
                throw new ConnectionDbException("User used as a paramater is null");
            }

            using(SqlConnection connection = new SqlConnection(GetConnectionString())) 
            {
                connection.Open();
                string sqlQuery = @$"INSERT INTO Listeners (Id, Email, Name, Password, Description, BirthDate, PhoneNumber) 
                                     VALUES (@id, @email, @name, @password, @description, @birthDate, @phoneNumber)";

                await connection.QueryAsync(sqlQuery, new 
                {
                    id = listener.Id, 
                    email = listener.Email, 
                    name = listener.Name,
                    password = listener.Password, 
                    description = listener.Description, 
                    birthDate = listener.BirthDate, 
                    phoneNumber = listener.PhoneNumber
                });
            }
        } 
    }
}
