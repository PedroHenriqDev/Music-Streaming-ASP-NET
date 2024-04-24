using DomainLayer.Entities;
using System.ComponentModel.DataAnnotations;
using UtilitiesLayer.Attributes;

namespace ApplicationLayer.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [StrongPassword(ErrorMessage = "The password must contain letters, numbers and special characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        public List<Genre> Genres { get; set; }

        public List<string> SelectedGenreIds { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        [DataType(DataType.Date)]
        [Age(ErrorMessage = "It's mandatory be at least 10 years old!")]
        public DateTime BirthDate { get; set; }

        [PhoneNumber(ErrorMessage = "The format of this number is incorrect")]
        public string PhoneNumber { get; set; }
    }
}
