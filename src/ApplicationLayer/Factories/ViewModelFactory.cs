using ApplicationLayer.ViewModels;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Factories
{
    public class ViewModelFactory
    {
        public DescriptionViewModel FactoryDescriptionViewModel<T>(T entity)
            where T : class, IEntityWithDescription<T>
        {
            if (entity == null)
            {
                throw new ArgumentNullException("It is impossible to manufacture objects that have null properties");
            }

            return new DescriptionViewModel(entity.Description, entity.Name, entity.Id);
        }

    }
}
