namespace DomainLayer.Interfaces
{
    public interface IJsonSerializationHelper
    {
        string SerializeObject(object obj);
        T DeserializeObject<T>(string json);
    }
}
