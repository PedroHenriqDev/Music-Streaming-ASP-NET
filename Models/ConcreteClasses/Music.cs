using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ConcreteClasses
{
    public class Music : IEntityWithName<Music>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ArtistId { get; set; }
        public string GenreId { get; set; }
        public DateTime Date {  get; set; }
        public DateTime DateCreation { get; set; }
        public Artist Artist { get; set; }
        public Genre Genre {  get; set; }

        public Music(string id, string name, string artistId, string genreId, DateTime date, DateTime dateCreation) 
        {
            Id = id;
            Name = name;
            ArtistId = artistId;
            GenreId = genreId;
            Date = date;
            DateCreation = dateCreation;
        }
    }
}
