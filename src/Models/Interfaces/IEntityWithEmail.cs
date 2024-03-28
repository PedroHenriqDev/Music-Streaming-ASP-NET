namespace Models.Interfaces
{
    public interface IEntityWithEmail<T> : IEntityWithName<T> 
    {
        public string Email { get; set; }
    }
}
