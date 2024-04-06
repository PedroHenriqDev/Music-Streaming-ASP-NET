using DomainLayer.Entities;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Factories
{
    public class ModelFactory
    {
        public T FacUser<T>(string id, string description)
            where T : class, IUser<T>, new()
        {
            return new T
            {
                Id = id,
                Description = description
            };
        }

        public T FacUser<T>(string id, string name, string email, string password, string phoneNumber, DateTime birthDate, DateTime dateCreation)
          where T : class, IUser<T>, new()
        {
            return new T
            {
                Id = id,
                Name = name,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
                BirthDate = birthDate,
                DateCreation = dateCreation
            };
        }

        public List<UserGenre<T>> FacUserGenres<T>(string id, List<string> ids)
            where T : class, IUser<T>
        {
            return ids.Select(genreId => new UserGenre<T> { Id = id, GenreId = genreId }).ToList();
        }
    }
}
