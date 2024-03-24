using MusicWeave.Models.AbstractClasses;
using MusicWeave.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MusicWeave.Models.ConcreteClasses
{
    public class Listener : User
    {
        public Listener() 
        {
        }

        public Listener(int id, string name, string password, string email, string phoneNumber, string description, DateTime birthDate)
            : base(id, name, password, email, phoneNumber, description, birthDate)
        {
        }
    }   
}
