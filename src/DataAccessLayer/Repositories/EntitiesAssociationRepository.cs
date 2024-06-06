using Dapper;
using DataAccessLayer.Sanitization;
using DataAccessLayer.Sql;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.Repositories
{
    public class EntitiesAssociationRepository
    {
        private readonly ConnectionDb _connectionDb;

        public EntitiesAssociationRepository(ConnectionDb connectionDb)
        {
            _connectionDb = connectionDb;
        }

        public async Task<IEnumerable<UserGenre<T>>> GetUserGenresAsync<T>(string id)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetAssociationTableGenre<T>()} WHERE Id = @id";
                return await connection.QueryAsync<UserGenre<T>>(sqlQuery, new { id = id });
            }
        }

        public async Task RecordPlaylistMusicsAsync(IEnumerable<PlaylistMusic> playlistMusics)
        {
            using (var connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $@"INSERT INTO PlaylistMusics (Id, PlaylistId, ListenerId, MusicId) VALUES (@id, @playlistId, @listenerId, @musicId)";

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var playlistMusic in playlistMusics)
                        {
                            await connection.ExecuteAsync(sqlQuery, new
                            {
                                id = playlistMusic.Id,
                                playlistId = playlistMusic.PlaylistId,
                                listenerId = playlistMusic.ListenerId,
                                musicId = playlistMusic.MusicId
                            },
                            transaction);
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new QueryException($"Failed to record playlist musics. Transaction rolled back because this error:{ex.Message}");
                    }
                }
            }
        }


        public async Task RecordUserGenresAsync<T>(List<UserGenre<T>> userGenres)
             where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();

                string tableName = TableNameSanitization.GetAssociationTableGenre<T>();
                string sqlQuery = $"INSERT INTO {tableName} (Id, GenreId) VALUES (@id, @genreId)";
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var userGenre in userGenres)
                        {
                            await connection.ExecuteAsync(sqlQuery, new
                            {
                                id = userGenre.Id,
                                genreId = userGenre.GenreId
                            });
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new RecordAssociationException($"An error unexpected error ocurred while genres were recorded, because this: {ex.Message}");
                    }
                }
            }
        }
    }
}
