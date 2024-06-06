using Dapper;
using DataAccessLayer.Sanitization;
using DataAccessLayer.Sql;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.Repositories
{
    public class UserRepository
    {
        private readonly ConnectionDb _connectionDb;

        public UserRepository(ConnectionDb connectionDb)
        {
            _connectionDb = connectionDb;
        }

        public async Task<T> GetUserByNameAsync<T>(string name)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = @$"
                                     SELECT 
                                        u.Id,
                                        u.Name,
                                        u.PictureProfile,
                                        u.Description
                                     FROM {TableNameSanitization.GetPluralTableName<T>()} u 
                                     WHERE
                                        Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task<T> GetUserByIdAsync<T>(string id)
            where T : class, IUser<T>

        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $@"
                                     SELECT 
                                        u.Id,
                                        u.name,
                                        u.PictureProfile,
                                        u.Description
                                     FROM {TableNameSanitization.GetPluralTableName<T>()} u 
                                     WHERE
                                        id = @id";

                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new
                {
                    id = id
                });
            }
        }

        public async Task RecordUserAsync<T>(T user)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
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


        public async Task UpdateUserProfilePictureAsync<T>(T user)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET PictureProfile = @pictureProfile WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new
                {
                    pictureProfile = user.PictureProfile,
                    id = user.Id
                });
            }
        }
    }
}
