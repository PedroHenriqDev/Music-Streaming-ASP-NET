using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MusicWeave.Models.Interfaces
{
    public interface IEntityWithName<T> : IEntity<T> where T : class
    {
        public string Name { get; set; }
    }
}
