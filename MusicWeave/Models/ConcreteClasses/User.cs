using MusicWeave.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MusicWeave.Models.ConcreteClasses
{
    public class User<T> : IEntity<User<T>>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password {  get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public DateTime BirthDate { get; set; }

        public User() 
        {
        }

        public User(int id, string username, string password, string email, string phoneNumber, string description, DateTime birthDate) 
        {
            Id = id;
            Username = username;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            Description = description;
            BirthDate = birthDate;
        }
    }
}
