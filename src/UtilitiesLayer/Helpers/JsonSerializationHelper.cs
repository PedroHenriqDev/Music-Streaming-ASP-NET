using Newtonsoft.Json;

namespace UtilitiesLayer.Helpers
{
    static public class JsonSerializationHelper
    {
        static public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        static public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
