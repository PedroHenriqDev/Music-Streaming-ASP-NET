using MusicWeave.Models.Interfaces;

namespace MusicWeave.Models.ConcreteClasses
{
    public class Artist : IUser<Artist>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public DateTime BirthDate { get; set; }

        public Artist()
        {
        }

        public Artist(int id, string name, string password, string email, string phoneNumber, string description, DateTime birthDate)
        {
            Id = id;
            Name = name;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            Description = description;
            BirthDate = birthDate;
        }
    }
}