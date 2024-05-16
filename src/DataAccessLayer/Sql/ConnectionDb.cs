using Dapper;
using DataAccessLayer.Sanitization;
using DomainLayer.Entities;
using DomainLayer.Exceptions;
using DomainLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DataAccessLayer.Sql
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

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>()
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()}";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByIdAsync<T>(string id)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                return await connection.QueryAsync<T>(sqlQuery, new { id = id });
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByIdsAsync<T>(IEnumerable<string> ids)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                var sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id IN ({FieldSanitization.JoinIds(ids)})";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByForeignKeyAsync<T, TR>(string fkId)
            where T : class, IEntity where TR : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = TableNameSanitization.GetPluralTableName<T>();
                string fkField = FieldSanitization.ForeignKeyName<TR>();
                var sqlQuery = $"SELECT * FROM {tableName} WHERE {fkField} = @fkid";
                return await connection.QueryAsync<T>(sqlQuery, new { fkId = fkId });
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesByForeignKeysAsync<T, TR>(IEnumerable<string> fkIds)
            where T : class, IEntity where TR : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string tableName = TableNameSanitization.GetPluralTableName<T>();
                string fieldFKName = FieldSanitization.ForeignKeyName<TR>();
                var sqlQuery = $"SELECT * FROM {tableName} WHERE {fieldFKName} IN ({FieldSanitization.JoinIds(fkIds)})";
                return await connection.QueryAsync<T>(sqlQuery);
            }
        }

        public async Task<T> GetEntityByCredentialsAsync<T>(string email, string password)
            where T : IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email AND Password = @password";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email, password = password });
            }
        }

        public T GetUserByName<T>(string name)
            where T : IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
                return connection.QueryFirst<T>(sqlQuery, new { name = name });
            }
        }

        public async Task<T> GetEntityByIdAsync<T>(string id)
            where T : IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { id = id });
            }
        }

        public async Task<T> GetUserByNameAsync<T>(string name)
            where T : IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task<T> GetEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Email = @email";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { email = email });
            }
        }

        public async Task<T> GetEntityByNameAsync<T>(string name)
            where T : class, IEntityWithName<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Name = @name";
                return await connection.QueryFirstOrDefaultAsync<T>(sqlQuery, new { name = name });
            }
        }

        public async Task<IEnumerable<UserGenre<T>>> GetUserGenresAsync<T>(string id)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM {TableNameSanitization.GetAssociationTableGenre<T>()} WHERE Id = @id";
                return await connection.QueryAsync<UserGenre<T>>(sqlQuery, new { id = id });
            }
        }
        public async Task<IEnumerable<Music>> GetMusicsByIdsAsync(List<string> ids)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $@"SELECT
                                m.Id, 
                                m.Name,
                                m.ArtistId,
                                m.GenreId,
                                m.Date,
                                m.DateCreation,
                                m.Duration,
                                a.Id AS ArtistId,
                                a.Name AS Name
                                FROM 
                                    Musics m
                                INNER JOIN 
                                    Artists a ON m.ArtistId = a.Id
                                WHERE
                                    m.Id IN ({FieldSanitization.JoinIds(ids)})";

                var result = await connection.QueryAsync<Music, Artist, Music>(
                    sqlQuery, (music, artist) =>
                    {
                        if (!(artist is null))
                        {
                            music.Artist = artist;
                            return music;
                        }
                        throw new QueryException("Error, artist null");
                    },
                    splitOn: "ArtistId");

                return result;
            }
        }

        public async Task<IEnumerable<Music>> GetMusicsByQueryAsync(string query)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $@"
                                    SELECT
                                        m.Id,
                                        m.Name,
                                        m.ArtistId,
                                        m.GenreId,
                                        m.Date,
                                        m.DateCreation,
                                        m.Duration,
                                        a.Id AS ArtistId,
                                        a.Name AS Name
                                    FROM
                                        Musics m
                                    INNER JOIN
                                        Artists a ON m.ArtistId = a.Id
                                    WHERE
                                         LOWER(m.Name) LIKE LOWER ('%' || @query || '%')
                                         OR LOWER (a.Name) LIKE LOWER ('%' || @query || '%')
                                         OR LOWER (a.Description) LIKE LOWER('%' || @query || '%')";

                var result = await connection.QueryAsync<Music, Artist, Music>(
                    sqlQuery,
                    (music, artist) =>
                    {
                        if (artist is null)
                        {
                            throw new QueryException("Error, not found artist");
                        }
                        music.Artist = artist;
                        return music;
                    },
                    splitOn: "ArtistId",
                    param: new { query });

                return result;
            }
        }

        public async Task<IEnumerable<Music>> GetMusicsByFkIdAsync<T>(string fkId)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string fkField = FieldSanitization.ForeignKeyName<T>();

                await connection.OpenAsync();
                string sqlQuery = $@"
                                   SELECT
                                       m.Id,
                                       m.Name,
                                       m.ArtistId,
                                       m.GenreId,
                                       m.Date,
                                       m.DateCreation,
                                       m.Duration,
                                       a.Id AS ArtistId,
                                       a.Name AS Name
                                    FROM
                                       Musics m
                                    INNER JOIN 
                                       Artists a ON m.ArtistId = a.Id
                                    WHERE 
                                       m.{fkField} = @fkId";

                var result = await connection.QueryAsync<Music, Artist, Music>
                    (sqlQuery,
                    (music, artist) =>
                    {
                        if (!(artist is null))
                        {
                            music.Artist = artist;
                            return music;
                        }
                        throw new QueryException("Not Found Artist!");
                    },
                    splitOn: "ArtistId",
                    param: new { fkId });

                return result;
            }
        }

        public async Task<IEnumerable<Music>> GetMusicsByFkIdsAsync<T>(IEnumerable<string> fkIds)
            where T : class, IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string fkField = FieldSanitization.ForeignKeyName<T>();

                await connection.OpenAsync();
                string sqlQuery = @$"
                                   SELECT
                                       m.Id,
                                       m.Name,
                                       m.ArtistId,
                                       m.GenreId,
                                       m.Date,
                                       m.DateCreation,
                                       m.Duration,
                                       a.Id AS ArtistId,
                                       a.Name AS Name
                                   FROM
                                       Musics m
                                   INNER JOIN
                                       Artists a ON m.ArtistId = a.Id
                                   WHERE
                                      m.{fkField} = ANY(@fkIds)";

                var result = await connection.QueryAsync<Music, Artist, Music>(
                        sqlQuery,
                        (music, artist) =>
                        {
                            if (!(artist is null))
                            {
                                music.Artist = artist;
                                return music;
                            }
                            throw new QueryException("Not Found Artists!");
                        },
                        splitOn: "ArtistId",
                        param: new { fkIds });

                return result;
            }
        }
        
        public async Task<IEnumerable<Playlist>> GetPlaylistsWithMusicsByListenerIdAsync(string listenerId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = @"
                            SELECT 
                                p.Id,
                                p.ListenerId,
                                p.Name,
                                p.Description,
                                p.CreateAt,
                                p.Image,
                                m.Id,
                                m.ArtistId,
                                m.Name,
                                m.DateCreation,
                                m.GenreId,
                                m.Date,
                                m.Duration,
                                a.Id,
                                a.Name,
                                a.PictureProfile,
                                l.Id,
                                l.Name,
                                l.Email,
                                l.PictureProfile
                            FROM 
                                Playlists p 
                            INNER JOIN
                                PlaylistMusic pm ON p.Id = pm.PlaylistId
                            INNER JOIN
                                Musics m ON pm.MusicId = m.Id
                            INNER JOIN
                                Artists a ON a.Id = m.ArtistId
                            INNER JOIN
                                Listeners l ON p.ListenerId = l.Id
                            WHERE 
                                p.ListenerId = @listenerId";

                var playlistsDictionary = new Dictionary<string, Playlist>();

                var result = await connection.QueryAsync<Playlist, Music, Artist, Listener, Playlist>(
                    sqlQuery,
                    (playlist, music, artist, listener) =>
                    {
                        if (!playlistsDictionary.TryGetValue(playlist.Id, out var playlistEntry))
                        {
                            playlistEntry = playlist;
                            playlistEntry.Listener = listener;
                            playlistEntry.Musics = new List<Music>();
                            playlistsDictionary.Add(playlist.Id, playlistEntry);
                        }
                        music.Artist = artist;
                        ((List<Music>)playlistEntry.Musics).Add(music);

                        return playlistEntry;
                    },
                    splitOn: "Id",
                    param: new { listenerId = listenerId }
                );

                return playlistsDictionary.Values;
            }
        }

        public async Task RecordUserAsync<T>(T user)
            where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
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

        public async Task RecordUserGenresAsync<T>(List<UserGenre<T>> userGenres)
             where T : class, IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
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
                    catch(Exception ex) 
                    {
                        await transaction.RollbackAsync();
                        throw new RecordAssociationException($"An error unexpected error ocurred while genres were recorded, because this: {ex.Message}");
                    }
                }
            }
        }

        public async Task RecordUserAndUserGenresAsync<T>(T user, List<UserGenre<T>> userGenres)
            where T : class, IUser<T>
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        await RecordUserAsync(user);
                        await RecordUserGenresAsync(userGenres);
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new RecordAssociationException(ex.Message);
                    }
                }
            }
        }

        public async Task RecordMusicAsync(Music music)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = @"INSERT INTO Musics (Id, Name, ArtistId, GenreId, Date, DateCreation, Duration) 
                                    VALUES(@id, @name, @artistId, @genreId, @date, @dateCreation, @duration)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = music.Id,
                    name = music.Name,
                    artistId = music.ArtistId,
                    genreId = music.GenreId,
                    date = music.Date,
                    dateCreation = music.DateCreation,
                    duration = music.Duration
                });
            }
        }

        public async Task RecordPlaylistMusicsAsync(IEnumerable<PlaylistMusic> playlistMusics)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $@"INSERT INTO PlaylistMusic (Id, PlaylistId, ListenerId, MusicId) VALUES (@id, @playlistId, @listenerId, @musicId)";

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var playlistMusic in playlistMusics)
                        {
                            await connection.ExecuteAsync(sqlQuery, new
                            {
                                id  = playlistMusic.Id,
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

        public async Task RecordMusicViewAsync(MusicView musicView) 
        {
                using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();
                    string sqlQuery = @"INSERT INTO MusicViews (Id, MusicId, ListenerId, CreatedAt) 
                                    VALUES (@id, @musicId, @listenerId, @createdAt)";

                    await connection.QueryAsync(sqlQuery, new
                    {
                        id = musicView.Id,
                        musicId = musicView.MusicId,
                        listenerId = musicView.ListenerId,
                        createdAt = musicView.CreatedAt
                    });
                }
        }

        public async Task RecordPlaylistAsync(Playlist playlist)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = @"INSERT INTO Playlists (Id, Visibility, ListenerId, Name, Image, CreateAt, Description) 
                   VALUES (@id, @visibility::visibilitytype, @listenerId, @name, @image, @createAt, @description)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = playlist.Id,
                    visibility = playlist.Visibility.ToString(),
                    name = playlist.Name,
                    listenerId = playlist.ListenerId,
                    image = playlist.Image,
                    createAt = playlist.CreateAt,
                    description = playlist.Description
                });
            }
        }

        public async Task UpdateUserProfilePictureAsync<T>(T user)
            where T : IUser<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET PictureProfile = @pictureProfile WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new { pictureProfile = user.PictureProfile, id = user.Id });
            }
        }

        public async Task UpdateDescriptionAsync<T>(T entity)
            where T : class, IEntityWithDescription<T>
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"UPDATE {TableNameSanitization.GetPluralTableName<T>()} SET Description = @description WHERE Id = @id";
                await connection.QueryAsync<T>(sqlQuery, new { description = entity.Description, id = entity.Id });
            }
        }

        public async Task DeleteEntityByIdAsync<T>(string id)
            where T : IEntity
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = $"DELETE FROM {TableNameSanitization.GetPluralTableName<T>()} WHERE Id = @id";
                await connection.QueryAsync(sqlQuery, new { id = id });
            }
        }
    }
}

