using System.ComponentModel.DataAnnotations;

namespace MusicWeave.Models.ViewModels
{
    public class LoginUserViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password {  get; set; }
    }
}
