using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Factories;
using ViewModels;

namespace Facades.Factories
{
    public class UserFactoriesFacade<T> where T : class, IUser<T>, new()
    {

        private readonly ViewModelFactory _viewModelFactory;
        private readonly ModelFactory _modelFactory;

        public UserFactoriesFacade(ViewModelFactory viewModelFactory, ModelFactory modelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _modelFactory = modelFactory;
        }

        public DescriptionViewModel FactoryDescriptionViewModel<TR>(T entity)
          where TR : class, IEntityWithDescription<TR>
        {
            return _viewModelFactory.FactoryDescriptionViewModel(entity);
        }

        public T FactoryUser(string id, string description)
        {
            return _modelFactory.FactoryUser<T>(id, description);
        }
    }
}
