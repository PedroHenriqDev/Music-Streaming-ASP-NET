using DomainLayer.Interfaces;

namespace DomainLayer.Entities
{
    public class Music : IEntityWithName<Music>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ArtistId { get; set; }
        public string GenreId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateCreation { get; set; }
        public Artist Artist { get; set; }
        public Genre Genre { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<MusicView> musicViews { get; set; }

        public Music() 
        {
        }

        public Music(string id, string name, string artistId, string genreId, DateTime date, DateTime dateCreation, TimeSpan duration)
        {
            Id = id;
            Name = name;
            ArtistId = artistId;
            GenreId = genreId;
            Date = date;
            DateCreation = dateCreation;
            Duration = duration;
        }
    }
}
