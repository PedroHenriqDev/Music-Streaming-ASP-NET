using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attributes;

namespace ViewModels
{
    public class AddMusicViewModel
    {
        [Required]
        public IFormFile Picture { get; set; }

        [Required]
        public IFormFile Audio { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "The date cannot be in the future!")]
        public DateTime Date { get; set; }

        [Required]
        public string GenreId { get; set; }

        public bool Step1IsValid 
        {
            get 
            {
                if(!string.IsNullOrEmpty(Name) && Date <= DateTime.Now && !string.IsNullOrEmpty(GenreId)) 
                {
                    return true;
                }
                return false;
            }
        }
    }
}
