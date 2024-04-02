using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.ConcreteClasses;
using Extensions;
using Attributes;

namespace ViewModels
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

        public bool Step1IsValid
        {
            get
            {
                DateTime birthDate = BirthDate;
                TimeSpan duration = DateTime.Now.Subtract(birthDate);

                return !string.IsNullOrEmpty(Name) &&
                       !string.IsNullOrEmpty(Email) &&
                       !string.IsNullOrEmpty(Password) &&
                       duration.TotalDays >= 3650;
            }
        }

        public bool Step2IsValid
        {
            get
            {
                return SelectedGenreIds != null && SelectedGenreIds.Any();
            }
        }
    }
}
