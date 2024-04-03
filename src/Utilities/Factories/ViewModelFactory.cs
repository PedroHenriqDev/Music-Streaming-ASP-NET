using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Utilities.Factories
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
