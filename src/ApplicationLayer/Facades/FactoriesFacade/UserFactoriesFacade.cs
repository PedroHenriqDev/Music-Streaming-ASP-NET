using ApplicationLayer.Factories;
using DomainLayer.Interfaces;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class UserFactoriesFacade<T> where T : class, IUser<T>, new()
    {
        private readonly ModelFactory _modelFactory;

        public UserFactoriesFacade(ModelFactory modelFactory) 
        {
            _modelFactory = modelFactory;
        }

        public T FacUser(string id, string description)
        {
            return _modelFactory.FacUser<T>(id, description);
        }
    }
}
