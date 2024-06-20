using DomainLayer.Interfaces;

namespace DomainLayer.Entities;

public class FavoritePlaylist : IEntity
{
    public string Id { get; set; }
    public string ListenerId { get; set; }
    public string PlaylistId { get; set; }

    public FavoritePlaylist() 
    {
    }

    public FavoritePlaylist(string id, string listenerId, string playlistId) 
    {
        Id = id;
        ListenerId = listenerId;
        PlaylistId = playlistId;
    }
}
