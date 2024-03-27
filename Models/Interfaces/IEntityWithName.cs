namespace Models.Interfaces
{
    public interface IEntityWithName<T> : IEntity
    {
        public string Name { get; set; }
    }
}
