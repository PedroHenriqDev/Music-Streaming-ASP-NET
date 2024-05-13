namespace DomainLayer.Entities
{
    public class MusicView
    {
        public string ListenerId { get; set; }
        public Listener Listener { get; set; }
        public string MusicId { get; set; }
        public Music Music { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
