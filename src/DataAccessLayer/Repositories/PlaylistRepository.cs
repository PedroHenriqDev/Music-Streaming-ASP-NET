using Dapper;
using DataAccessLayer.Mappers;
using DomainLayer.Entities;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using Npgsql;

namespace DataAccessLayer.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly DataMapper _mapper;

        public PlaylistRepository(NpgsqlConnection connection,
                                  DataMapper dataMapper)
        {
            _connection = connection;
            _mapper = dataMapper;
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsWithMusicsByListenerIdAsync(string listenerId)
        {
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
            var result = await _connection.QueryAsync<Playlist, Music, Artist, Listener, Playlist>(
                sqlQuery,
                (playlist, music, artist, listener) =>
                {
                    return _mapper.MapPlaylistDictionary(playlist, playlistsDictionary, music, listener);
                },
                splitOn: "Id",
                param: new { listenerId = listenerId });

            return playlistsDictionary.Values;
        }

        public async Task<Playlist> GetPlaylistByIdAsync(string playlistId)
        {
            string sqlQuery = @"
                                SELECT 
                                   p.Id,
                                   p.Description,
                                   p.Image,
                                   p.Name,
                                   p.CreateAt,
                                   p.ListenerId,
                                   p.Visibility,
                                   pm.PlaylistId,
                                   pm.MusicId,
                                   m.Id,
                                   m.Name,
                                   m.Date,
                                   m.DateCreation,
                                   m.ArtistId,
                                   m.Duration,
                                   a.Id AS ArtistId,
                                   a.Name,
                                   l.Id AS ListenerId,
                                   l.Name
                                FROM 
                                   PlaylistMusics pm 
                                INNER JOIN 
                                   Playlists p ON p.Id = pm.PlaylistId
                                INNER JOIN
                                   Musics m ON m.Id = pm.MusicId
                                    INNER JOIN 
                                        Artists a ON a.Id = m.ArtistId
                                    INNER JOIN
                                        Listeners l ON l.Id = p.ListenerId
                                    WHERE
                                        p.Id = @playlistId";

            var playlistDictionary = new Dictionary<string, Playlist>();
            var result = await _connection.QueryAsync<Playlist, Music, Listener, Artist, Playlist>(sqlQuery,
                (playlist, music, listener, artist) =>
                {
                    return _mapper.MapPlaylistDictionary(playlist, playlistDictionary, _mapper.MapMusicArtist(music, artist), listener);
                },
                splitOn: "Id,ArtistId,ListenerId",
                param: new { playlistId });

            return playlistDictionary.Values.FirstOrDefault();
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsByQueryAsync(string query)
        {
            string sqlQuery = @"
                                SELECT 
                                    p.Id,
                                    p.Image, 
                                    p.Name,
                                    p.CreateAt,
                                    p.ListenerId,
                                    p.Visibility,
                                    p.Description,
                                    l.Id AS ListenerId,
                                    l.Name AS ListenerName,
                                    pm.PlaylistId,
                                    pm.MusicId,
                                    m.Id AS MusicId,
                                    m.Name AS MusicName
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

            var result = await _connection.QueryAsync<Playlist, Listener, PlaylistMusic, Music, Playlist>(
                sqlQuery,
                (playlist, listener, playlistMusic, music) =>
                {
                    return _mapper.MapPlaylistDictionary(playlist, playlistDictionary, music, listener);
                },
                param: new { query = query, visibility = VisibilityType.Public.ToString().ToLower() },
                splitOn: "ListenerId,MusicId");

            return playlistDictionary.Values.ToList();
        }

        public async Task RecordPlaylistAsync(Playlist playlist)
        {
                string sqlQuery = @"INSERT INTO Playlists (Id, Visibility, ListenerId, Name, Image, CreateAt, Description) 
                   VALUES (@id, @visibility::visibilitytype, @listenerId, @name, @image, @createAt, @description)";

                await _connection.QueryAsync(sqlQuery, new
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
