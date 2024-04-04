using Models.Entities;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Factories
{
    public class ModelFactory
    {
        public T FactoryUser<T>(string id, string description)
            where T : class, IUser<T>, new()
        {
            return new T 
            {
                Id = id,
                Description = description 
            };
        }

        public T FactoryUser<T>(string id, string name, string email, string password, string phoneNumber, DateTime birthDate, DateTime dateCreation)
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

        public List<UserGenre<T>> FactoryUserGenres<T>(string id, List<string> ids)
            where T : class, IUser<T>
        {
            List<UserGenre<T>> userGenres = ids.Select(genreId => new UserGenre<T> { Id = id, GenreId = genreId }).ToList();
            return userGenres;
        }
    }
}
