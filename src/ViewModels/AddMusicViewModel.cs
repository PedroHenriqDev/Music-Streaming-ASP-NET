using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AddMusicViewModel
    {
        public IFormFile MusicPicture { get; set; }
        public IFormFile MusicData { get; set; } 
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string GenreId { get; set; }
        public string ArtistId { get; set; }
    }
}
