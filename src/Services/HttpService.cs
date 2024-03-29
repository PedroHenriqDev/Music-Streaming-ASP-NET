using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class HttpService
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly JsonSerializationService _jsonService;

        public HttpService(
            IHttpContextAccessor httpAccessor, 
            JsonSerializationService jsonSerialization)
        {   
            _httpAccessor = httpAccessor;
            _jsonService = jsonSerialization;
        }

        public T GetSessionValue<T>(string key) 
        {
            string value = _httpAccessor.HttpContext.Session.GetString(key);
            return value != null ? _jsonService.DeserializeObject<T>(value) : default(T);
        }

        public void SetSessionValue<T>(string key, T value) 
        {
            string serializeValue = _jsonService.SerializeObject(value);
            _httpAccessor.HttpContext.Session.SetString(key, serializeValue);
        } 

        public void RemoveSessionValue(string key)
        {
            _httpAccessor.HttpContext.Session.Remove(key);
        }
    }
}
