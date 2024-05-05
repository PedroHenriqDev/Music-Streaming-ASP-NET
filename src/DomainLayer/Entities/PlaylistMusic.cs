namespace DomainLayer.Entities
{
    public class PlaylistMusic
    {
        public string Id { get; set; }
        public string PlaylistId { get; set; }
        public string ListenerId { get; set; }
        public string MusicId { get; set; }
    }
}
