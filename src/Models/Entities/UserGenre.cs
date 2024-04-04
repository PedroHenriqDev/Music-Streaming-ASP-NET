using Models.Interfaces;

namespace Models.Entities
{
    public class UserGenre<T> where T : class, IUser<T>
    {
        public string Id { get; set; }
        public string GenreId { get; set; }
    }
}
