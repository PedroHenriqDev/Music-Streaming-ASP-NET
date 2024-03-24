using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MusicWeave.Models.Interfaces
{
    public interface IEntityWithName<T> : IEntity
    {
        public string Name { get; set; }
    }
}
