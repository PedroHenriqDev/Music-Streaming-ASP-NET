using DomainLayer.Interfaces;

namespace DomainLayer.Entities
{
    public class PlaylistMusic : IEntity
    {
        public string Id { get; set; }
        public string MusicId { get; set; }
    }
}
