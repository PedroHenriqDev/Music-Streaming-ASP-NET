using MusicWeave.Models.ConcreteClasses;
using MusicWeave.Models.Interfaces;
using MusicWeave.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MusicWeave.Models.ViewModels
{
    public class RegisterListenerViewModel : Listener
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Age(ErrorMessage = "It's mandatory be at least 10 years old!")]
        public DateTime BirthDate { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
