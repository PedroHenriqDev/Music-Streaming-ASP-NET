using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Sql
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
    }
}

