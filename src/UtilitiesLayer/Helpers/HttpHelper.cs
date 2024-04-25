using Microsoft.AspNetCore.Http;
using System.Text;

namespace UtilitiesLayer.Helpers
{
    static public class HttpHelper
    {
        static public T GetSessionValue<T>(IHttpContextAccessor httpAccessor, string key)
        {
            string value = httpAccessor.HttpContext.Session.GetString(key);
            return value != null ? JsonSerializationHelper.DeserializeObject<T>(value) : default(T);
        }

        static public void SetSessionValue<T>(IHttpContextAccessor httpAccessor, string key, T value)
        {
            string serializeValue = JsonSerializationHelper.SerializeObject(value);
            httpAccessor.HttpContext.Session.SetString(key, serializeValue);
        }

        static public void RemoveSessionValue(IHttpContextAccessor httpAccessor, string key)
        {
            httpAccessor.HttpContext.Session.Remove(key);
        }
    }
}
