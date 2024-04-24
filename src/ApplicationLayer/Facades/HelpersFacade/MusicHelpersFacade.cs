using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Facades.HelpersFacade
{
    public class MusicHelpersFacade
    {
        private readonly JsonSerializationHelper _jsonHelper;

        public MusicHelpersFacade(JsonSerializationHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public string SerializeObject(object obj) 
        {
            return _jsonHelper.SerializeObject(obj);
        }
    }
}
