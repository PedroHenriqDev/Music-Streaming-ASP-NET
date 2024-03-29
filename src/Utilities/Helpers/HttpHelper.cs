using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class HttpHelper
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly JsonSerializationHelper _jsonHelper;

        public HttpHelper(
            IHttpContextAccessor httpAccessor, 
            JsonSerializationHelper jsonHelper)
        {   
            _httpAccessor = httpAccessor;
            _jsonHelper = jsonHelper;
        }

        public T GetSessionValue<T>(string key) 
        {
            string value = _httpAccessor.HttpContext.Session.GetString(key);
            return value != null ? _jsonHelper.DeserializeObject<T>(value) : default(T);
        }

        public void SetSessionValue<T>(string key, T value) 
        {
            string serializeValue = _jsonHelper.SerializeObject(value);
            _httpAccessor.HttpContext.Session.SetString(key, serializeValue);
        } 

        public void RemoveSessionValue(string key)
        {
            _httpAccessor.HttpContext.Session.Remove(key);
        }
    }
}
