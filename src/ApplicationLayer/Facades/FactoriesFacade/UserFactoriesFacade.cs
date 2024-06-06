using ApplicationLayer.Factories;
using DomainLayer.Entities;
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

        public List<UserGenre<T>> FacUserGenres(string userId, List<string> genreIds)
        {
            return _modelFactory.FacUserGenres<T>(userId, genreIds);
        }
    }
}
