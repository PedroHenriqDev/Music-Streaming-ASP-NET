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
                Id = id, Description = description 
            };
        }
    }
}
