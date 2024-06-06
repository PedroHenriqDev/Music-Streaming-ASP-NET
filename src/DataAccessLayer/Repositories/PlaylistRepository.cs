using Dapper;
using DataAccessLayer.Mappers;
using DataAccessLayer.Sql;
using DomainLayer.Entities;
using DomainLayer.Enums;
using Npgsql;

namespace DataAccessLayer.Repositories
{
    public class PlaylistRepository
    {
        private readonly DataMapper _mapper;
        private readonly ConnectionDb _connectionDb;
        
        public PlaylistRepository(ConnectionDb connectionDb, DataMapper dataMapper)
        {
            _connectionDb = connectionDb;
            _mapper = dataMapper;
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsWithMusicsByListenerIdAsync(string listenerId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
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
                                l.PictureProfile
                            FROM 
                                Playlists p 
                            INNER JOIN
                                PlaylistMusics pm ON p.Id = pm.PlaylistId
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
                        return _mapper.MapPlaylist(playlist, playlistsDictionary, music, listener);
                    },
                    splitOn: "Id",
                    param: new { listenerId = listenerId }
                );

                return playlistsDictionary.Values;
            }
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsByQueryAsync(string query)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = @"
                                    SELECT 
                                        p.Id,
                                        p.Image, 
                                        p.Name,
                                        p.CreateAt,
                                        p.ListenerId,
                                        p.Visibility,
                                        p.Description,
                                        l.Id as ListenerId,
                                        l.Name as ListenerName,
                                        pm.PlaylistId,
                                        pm.MusicId,
                                        m.Id as MusicId,
                                        m.Name as MusicName
                                    FROM 
                                        Playlists p
                                    INNER JOIN
                                        Listeners l ON p.ListenerId = l.Id
                                    INNER JOIN
                                        PlaylistMusics pm ON pm.PlaylistId = p.Id
                                    INNER JOIN
                                        Musics m ON m.Id = pm.MusicId
                                    WHERE 
                                        (LOWER(p.Name) LIKE LOWER('%' || @query || '%') 
                                        OR LOWER(p.Description) LIKE LOWER('%' || @query || '%') 
                                        OR LOWER(l.Name) LIKE LOWER('%' || @query || '%'))
                                        AND p.Visibility = CAST(@visibility AS visibilitytype)";

                var playlistDictionary = new Dictionary<string, Playlist>();

                var result = await connection.QueryAsync<Playlist, Listener, PlaylistMusic, Music, Playlist>(
                    sqlQuery,
                    (playlist, listener, playlistMusic, music) =>
                    {
                        return _mapper.MapPlaylist(playlist, playlistDictionary,music, listener);
                    },
                    param: new { query = query, visibility = VisibilityType.Public.ToString().ToLower() },
                    splitOn: "ListenerId,MusicId");

                return playlistDictionary.Values.ToList();
            }
        }

        public async Task RecordPlaylistAsync(Playlist playlist)
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionDb.GetConnectionString()))
            {
                await connection.OpenAsync();
                string sqlQuery = @"INSERT INTO Playlists (Id, Visibility, ListenerId, Name, Image, CreateAt, Description) 
                   VALUES (@id, @visibility::visibilitytype, @listenerId, @name, @image, @createAt, @description)";

                await connection.QueryAsync(sqlQuery, new
                {
                    id = playlist.Id,
                    visibility = playlist.Visibility.ToString().ToLower(),
                    name = playlist.Name,
                    listenerId = playlist.ListenerId,
                    image = playlist.Image,
                    createAt = playlist.CreateAt,
                    description = playlist.Description
                });
            }
        }
    }
}
