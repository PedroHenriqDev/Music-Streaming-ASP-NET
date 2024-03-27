using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ConcreteClasses
{
    public class ArtistGenres
    {
        public string ArtistId { get; set; }
        public string GenreId { get; set;}

        public ArtistGenres(string artistId, string genreId) 
        {
            ArtistId = artistId;
            GenreId = genreId;
        }
    }
}
