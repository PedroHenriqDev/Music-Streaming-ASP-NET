using DomainLayer.Interfaces;

namespace DomainLayer.Entities
{
    public class Playlist : IEntityWithDescription<Playlist>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}
