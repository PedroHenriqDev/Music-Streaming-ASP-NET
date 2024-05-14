using ApplicationLayer.Factories;
using DomainLayer.Entities;

namespace ApplicationLayer.Facades.FactoriesFacade
{
    public class MusicFactoriesFacade
    {
        private readonly ModelFactory _modelFactory;
        public MusicFactoriesFacade(ModelFactory modelFactory)
        {
            _modelFactory = modelFactory;
        }

        public MusicView FacMusicView(string id, string listenerId, string musicId, DateTime createdAt)
        {
            return _modelFactory.FacMusicView(id, listenerId, musicId, createdAt);
        }
    }
}
