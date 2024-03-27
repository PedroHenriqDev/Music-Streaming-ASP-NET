using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IJsonSerializationService
    {
        string SerializeObject(object obj);
        T DeserializeObject<T>(string json);
    }
}
