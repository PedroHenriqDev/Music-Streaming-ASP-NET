using MusicWeave.Models.Interfaces;

namespace MusicWeave.Models.AbstractClasses
{
    public class User : IEntityWithEmail<User>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public byte[]? PictureProfile { get; set; }
        public string UserType { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DateCreation {  get; set; }

        public User()
        {
        }

        protected User(string userType)
        {
            UserType = userType;
        }

        protected User(int id, string name, string password, string email, string phoneNumber, string userType, DateTime birthDate, DateTime dateCreation)
        {
            Id = id;
            Name = name;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            UserType = userType;
            BirthDate = birthDate;
            DateCreation = dateCreation;
        }
    }
}
