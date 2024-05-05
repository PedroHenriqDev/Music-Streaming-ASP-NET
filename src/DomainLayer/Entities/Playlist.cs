using DomainLayer.Interfaces;

namespace DomainLayer.Entities
{
    public class Playlist : IEntityWithDescription<Playlist>
    {
        public string Id { get; set; }
        public string ListenerId { get; set; }
        public Listener Listener { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public DateTime CreateAt {  get; set; }
        public IEnumerable<Music> Musics { get; set; }

        public Playlist() 
        {
        }

        public Playlist(string id, string listenerId, string name,  string description, byte[] image, DateTime createdAt)
        {
            Id = id;
            ListenerId = listenerId;
            Name = name;
            Description = description;
            Image = image;
            CreateAt = createdAt;
        }

        public Playlist(string id, string listenerId, Listener listener, string name, string description, byte[] image, DateTime createdAt)
        {
            Id = id;
            ListenerId = listenerId;
            Listener = listener;
            Name = name;
            Description = description;
            Image = image;
            CreateAt = createdAt;
        }
    }
}
