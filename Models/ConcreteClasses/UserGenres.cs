using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ConcreteClasses
{
    public class UserGenres
    {
        public string Id { get; set; } 
        public string UserId { get; set; }
        public string GenreId { get; set;}

        public UserGenres(string id,string userId, string genreId) 
        {
            UserId = userId;
            GenreId = genreId;
            Id = id;
        }
    }
}
