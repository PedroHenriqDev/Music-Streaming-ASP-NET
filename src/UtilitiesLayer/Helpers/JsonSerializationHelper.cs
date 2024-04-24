using DomainLayer.Interfaces;
using Newtonsoft.Json;

namespace UtilitiesLayer.Helpers
{
    public class JsonSerializationHelper : IJsonSerializationHelper
    {
        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
