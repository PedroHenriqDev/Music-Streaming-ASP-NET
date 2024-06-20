namespace DomainLayer.Entities;

public class EntityVerify<T>
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public T Entity { get; set; }

    public EntityVerify()
    {
    }

    public EntityVerify(bool isValid, string message, T entity)
    {
        IsValid = isValid;
        Message = message;
        Entity = entity;
    }
}
