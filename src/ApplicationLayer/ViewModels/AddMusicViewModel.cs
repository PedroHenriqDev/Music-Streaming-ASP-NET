using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using UtilitiesLayer.Attributes;

namespace ApplicationLayer.ViewModels
{
    public class AddMusicViewModel
    {
        [Required]
        public IFormFile PictureFile { get; set; }

        [Required]
        public IFormFile AudioFile { get; set; }

        [Required(ErrorMessage = "Music name is a required field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Music date is a required field")]
        [FutureDate(ErrorMessage = "The date cannot be in the future!")]
        public DateTime Date { get; set; }

        [Required]
        public string GenreId { get; set; }
    }
}
