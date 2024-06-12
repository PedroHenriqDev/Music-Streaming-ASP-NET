using Dapper;
using DataAccessLayer.Mappers;
using DataAccessLayer.Sanitization;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.Repositories
{
    public class EntitiesAssociationRepository : IEntitiesAssociationRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly DataMapper _mapper;

        public EntitiesAssociationRepository(NpgsqlConnection connection,
                                             DataMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserGenre<T>>> GetUserGenresAsync<T>(string id)
            where T : class, IUser<T>
        {
            string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetAssociationTableGenre<T>()} WHERE Id = @id";
            return await _connection.QueryAsync<UserGenre<T>>(sqlQuery, new { id = id });
        }

        public async Task RecordPlaylistMusicsAsync(IEnumerable<PlaylistMusic> playlistMusics)
        {
            using (var transaction = await _connection.BeginTransactionAsync())
            {
                string sqlQuery = $@"INSERT INTO PlaylistMusics (Id, PlaylistId, ListenerId, MusicId) VALUES (@id, @playlistId, @listenerId, @musicId)";
                try
                {
                    foreach (var playlistMusic in playlistMusics)
                    {
                        await _connection.ExecuteAsync(sqlQuery, new
                        {
                            id = playlistMusic.Id,
                            playlistId = playlistMusic.PlaylistId,
                            listenerId = playlistMusic.ListenerId,
                            musicId = playlistMusic.MusicId
                        }, transaction);
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


        public async Task RecordUserGenresAsync<T>(List<UserGenre<T>> userGenres)
             where T : class, IUser<T>
        {
            using (var transaction = await _connection.BeginTransactionAsync())
            {
                string tableName = TableNameSanitization.GetAssociationTableGenre<T>();
                string sqlQuery = $"INSERT INTO {tableName} (Id, GenreId) VALUES (@id, @genreId)";

                try
                {
                    foreach (var userGenre in userGenres)
                    {
                        await _connection.ExecuteAsync(sqlQuery, new
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
