namespace MusicWeave.Models.Interfaces
{
    public interface IEntity<T> where T : class
    {
        int Id { get; }
    }
}
