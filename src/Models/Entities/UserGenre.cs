using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class UserGenre
    {
        public string UserId { get; set; }
        public string GenreId { get; set; }

        public UserGenre(string userId, string genreId)
        {
            UserId = userId;
            GenreId = genreId;
        }
    }
}
