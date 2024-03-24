using MusicWeave.Models.AbstractClasses;

namespace MusicWeave.Models.ConcreteClasses
{
    public class Artist : User
    {
        public Artist()
        {
        }

        public Artist(int id, string name, string password, string email, string phoneNumber, string description, DateTime birthDate)
           : base(id, name, password, email, phoneNumber, description, birthDate)
        {
        }
    }
}
