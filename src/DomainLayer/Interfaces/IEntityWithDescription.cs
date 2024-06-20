namespace DomainLayer.Interfaces;

public interface IEntityWithDescription<T> : IEntityWithName<T>
{
    public string Description { get; set; }
}
