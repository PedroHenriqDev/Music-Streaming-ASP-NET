using Attributes;
using Models.ConcreteClasses;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class RegisterArtistViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [StrongPassword(ErrorMessage = "The password must contain letters, numbers and special characters")]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public List<string> GenreIds { get; set; }

        [Required]
        [Age(ErrorMessage = "It's mandatory be at least 10 years old!")]
        public DateTime BirthDate { get; set; }

        [PhoneNumber(ErrorMessage = "The format of this number is incorrect")]
        public string? PhoneNumber { get; set; }
    }
}
