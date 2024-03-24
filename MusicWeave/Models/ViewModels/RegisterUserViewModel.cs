using System.ComponentModel.DataAnnotations;

namespace MusicWeave.Models.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public string PhoneNumber { get; set; }
    }
}
