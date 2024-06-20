using DomainLayer.Interfaces;

namespace DomainLayer.Entities;

public class MusicView : IEntity
{
    public string Id { get; set; }
    public string ListenerId { get; set; }
    public Listener Listener { get; set; }
    public string MusicId { get; set; }
    public Music Music { get; set; }
    public DateTime CreatedAt { get; set; }

    public MusicView() 
    {
    }  

    public MusicView(string id, string listenerId, string musicId, DateTime createdAt)
    {
        Id = id;
        ListenerId = listenerId;
        MusicId = musicId;
        CreatedAt = createdAt;
    }
}
