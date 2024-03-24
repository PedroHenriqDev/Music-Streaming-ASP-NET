using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MusicWeave.Models.Interfaces
{
    public interface IEntity<T>
    {
        public int Id { get; set; }
    }
}
