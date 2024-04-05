using DomainLayer.Interfaces;

namespace DomainLayer.Entities
{
    public class Genre : IEntityWithName<Genre>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }

        public Genre(string id, string name, string description, string date)
        {
            Id = id;
            Name = name;
            Description = description;
            Date = date;
        }

        public Genre()
        {
        }
    }
}
