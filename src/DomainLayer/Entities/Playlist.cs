using DomainLayer.Interfaces;

namespace DomainLayer.Entities
{
    public class Playlist : IEntityWithDescription<Playlist>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public DateTime CreateAt {  get; set; }

        public Playlist() 
        {
        }

        public Playlist(string id, string name, string description, byte[] image, DateTime createdAt)
        {
            Id = id;
            Name = name;
            Description = description;
            Image = image;
            CreateAt = createdAt;
        }
    }
}
