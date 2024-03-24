using System.Data.SqlClient;
using Dapper;
using MusicWeave.Exceptions;
using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.Interfaces;

namespace MusicWeave.Data
{
    public class ConnectionDb
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConnectionDb> _logger;

        public ConnectionDb(IConfiguration configuration, ILogger<ConnectionDb> logger) 
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
                throw new DbException("Object used as a parameter is null");
            }

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string tableName = typeof(T).Name + "s";
                string sqlQuery = $"SELECT * FROM {tableName} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = entity.Name });
            }
        }

        public async Task<User> GetUserByEmailAsync(string email) 
        {
            if(email == null) 
            {
                _logger.LogWarning("At the time of query the email was null");
                throw new DbException("Email used as a paramater is null");
            }

            using(SqlConnection connection = new SqlConnection(GetConnectionString())) 
            {
                connection.Open();
                string sqlQuery = $"SELECT * FROM Users WHEre Email = @email";
                return await connection.QueryFirstOrDefaultAsync<User>(sqlQuery, new {email = email});
            }
        }
    }
}
