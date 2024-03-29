using Attributes;
using Extensions;
using Models.ConcreteClasses;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class RegisterUserViewModel
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

        public bool UserIsValid
        {
            get
            {
                DateTime birthDate = BirthDate;
                TimeSpan duration = DateTime.Now.Subtract(birthDate);

                if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && duration.TotalDays >= 3650)
                {
                    return true;
                }
                return false;
            }
        }

        public bool GenreIdsIsValid 
        {
            get 
            {
                if (GenreIds != null && GenreIds.Any()) 
                {
                    return true;
                }
                return false;
            }
        }
    }
}
