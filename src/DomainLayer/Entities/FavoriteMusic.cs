using DomainLayer.Interfaces;

namespace DomainLayer.Entities;

public class FavoriteMusic : IEntity
{
    public string Id { get; set; }
    public string ListenerId { get; set; }
    public Listener Listener { get; set; }
    public string MusicId { get; set; }
    public Music Music { get; set; }

    public FavoriteMusic(string id, string musicId, string listenerId)
    {
        Id = id;
        MusicId = musicId;
        ListenerId = listenerId;
    }

    public FavoriteMusic() 
    {
    }  
}
