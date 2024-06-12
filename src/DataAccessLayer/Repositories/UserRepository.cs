using Dapper;
using DataAccessLayer.Mappers;
using DataAccessLayer.Sanitization;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly DataMapper _mapper;

        public UserRepository(NpgsqlConnection connection, 
                              DataMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<T> GetUserByNameAsync<T>(string name)
            where T : class, IUser<T>
        {
            string sqlQuery = @$"
                                 SELECT 
                                    u.Id,
                                    u.Name,
                                    u.PictureProfile,
                                    u.Description
                                 FROM
                                    {TableNameSanitization.GetPluralTableName<T>()} u 
                                 WHERE
                                    Name = @name";
            return await _connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name });
        }

        public async Task<T> GetUserByCredentialsAsync<T>(string email, string password)
           where T : IUser<T>
        {
            string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email AND Password = @password";
            return await _connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new
            {
                email,
                password
            });
        }

        public async Task<T> GetUserByIdAsync<T>(string id)
            where T : class, IUser<T>
        {
            string sqlQuery = $@"
                                 SELECT 
                                    u.Id,
                                    u.name,
                                    u.PictureProfile,
                                    u.Description
                                 FROM {TableNameSanitization.GetPluralTableName<T>()} u 
                                 WHERE
                                    id = @id";

            return await _connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new
            {
                id
            });
        }

        public async Task RecordUserAsync<T>(T user)
            where T : class, IUser<T>
        {
            string sqlQuery = $@"INSERT INTO {TableNameSanitization.GetPluralTableName<T>()} (Id, Email, Name, Password, Description, BirthDate, PictureProfile, PhoneNumber, DateCreation) 
                                 VALUES (@id, @name, @email, @password, @description, @birthDate, @pictureProfile, @phoneNumber, @dateCreation)";

            await _connection.ExecuteAsync(sqlQuery, new
            {
                user.Id,
                user.Email,
                user.Name,
                user.Password,
                user.Description,
                user.BirthDate,
                user.PictureProfile,
                user.PhoneNumber,
                user.DateCreation,
            });
        }

        public async Task UpdateUserProfilePictureAsync<T>(T user)
            where T : class, IUser<T>
        {
            string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET PictureProfile = @pictureProfile WHERE Id = @id";
            await _connection.ExecuteAsync(sqlQuery, new
            {
                user.PictureProfile,
                user.Id
            });
        }
    }
}
