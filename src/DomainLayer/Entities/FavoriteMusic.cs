namespace DomainLayer.Entities
{
    public class FavoriteMusic
    {
        public string ListenerId { get; set; }
        public Listener Listener { get; set; }
        public string MusicId { get; set; }
        public Music Music { get; set; }

        public FavoriteMusic(string musicId, string listenerId)
        {
            MusicId = musicId;
            ListenerId = listenerId;
        }

        public FavoriteMusic() 
        {
        }  
    }
}
