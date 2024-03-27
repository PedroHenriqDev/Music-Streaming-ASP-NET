using Models.Interfaces;

namespace Models.ConcreteClasses
{
    public class Artist : IUser<Artist>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public byte[] PictureProfile {  get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime DateCreation {  get; set; }
        public List<ArtistGenres>? ArtistsGenres;

        public Artist()
        {
        }

        public Artist(
            string id, 
            string name, 
            string password, 
            string email, 
            string phoneNumber, 
            DateTime birthDate, 
            DateTime dateCreation)
        {
            Id = id;
            Name = name;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
            DateCreation = dateCreation;
        }
    }
}