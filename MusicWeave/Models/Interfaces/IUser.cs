namespace MusicWeave.Models.Interfaces
{
    public interface IUser<T> : IEntityWithName<T> where T : class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
