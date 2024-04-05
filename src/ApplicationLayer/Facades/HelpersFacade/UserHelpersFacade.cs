using DomainLayer.Interfaces;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Facades.HelpersFacade
{
    public class UserHelpersFacade<T> where T : class, IUser<T>
    {
        private readonly HttpHelper _httpHelper;

        public UserHelpersFacade(HttpHelper httpHelper) 
        {
            _httpHelper = httpHelper;
        }

        public void SetSessionValue<TR>(string key, TR value)
        {
            _httpHelper.SetSessionValue(key, value);
        }

        public TR GetSessionValue<TR>(string key)
        {
            return _httpHelper.GetSessionValue<TR>(key);
        }

        public void RemoveSessionValue(string key)
        {
            _httpHelper.RemoveSessionValue(key);
        }
    }
}
