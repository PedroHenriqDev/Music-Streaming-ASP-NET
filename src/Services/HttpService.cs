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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializationService _jsonService;

        public HttpService(
            IHttpContextAccessor httpContextAccessor, 
            JsonSerializationService jsonSerialization)
        {   
            _httpContextAccessor = httpContextAccessor;
            _jsonService = jsonSerialization;
        }

        public T GetSessionValue<T>(string key) 
        {
            string value = _httpContextAccessor.HttpContext.Session.GetString(key);
            return value != null ? _jsonService.DeserializeObject<T>(value) : default(T);
        }

        public void SetSessionValue<T>(string key, T value) 
        {
            string serializeValue = _jsonService.SerializeObject(value);
            _httpContextAccessor.HttpContext.Session.SetString(key, serializeValue);
        } 

        public void RemoveSessionValue(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }
    }
}
