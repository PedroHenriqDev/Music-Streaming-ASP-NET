using System.Data.SqlClient;
using Dapper;

using MusicWeave.Models.Interfaces;

namespace MusicWeave.Data
{
    public class ConnectionDb
    {
        private readonly IConfiguration _configuration;

        public ConnectionDb(IConfiguration configuration) 
        {
            _configuration = configuration;
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
    }
}
