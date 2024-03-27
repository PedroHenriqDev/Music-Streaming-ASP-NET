using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IUser<T> : IEntityWithEmail<T> 
    {
        string Id { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        string? PhoneNumber { get; set; }
        string? Description { get; set; }
        byte[] PictureProfile { get; set; }
        DateTime BirthDate { get; set; }
        DateTime DateCreation { get; set; }
    }
}
