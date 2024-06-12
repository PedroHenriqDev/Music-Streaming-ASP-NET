namespace DomainLayer.Interfaces
{
    public interface IGenericRepository
    {
        Task<IEnumerable<T>> GetAllEntitiesAsync<T>()
            where T : class, IEntity;


        Task<IEnumerable<T>> GetEntitiesByIdAsync<T>(string id) 
            where T : class, IEntity;


        Task<IEnumerable<T>> GetEntitiesByIdsAsync<T>(IEnumerable<string> ids) 
            where T : class, IEntity;

        Task<IEnumerable<TTable>> GetEntitiesByFKAsync<TTable, TField>(string fkId) 
            where TTable : class, IEntity where TField : class, IEntity;

        Task<IEnumerable<TTable>> GetEntitiesByForeignKeysAsync<TTable, TField>(IEnumerable<string> fkIds)
            where TTable : class, IEntity where TField : class, IEntity;

        Task<T> GetEntityByIdAsync<T>(string id)
         where T : IEntity;
     

        Task<T> GetEntityByEmailAsync<T>(string email)
            where T : IEntityWithEmail<T>;
       

        Task<T> GetEntityByNameAsync<T>(string name)
            where T : class, IEntityWithName<T>;
      

        Task UpdateDescriptionAsync<T>(T entity)
            where T : class, IEntityWithDescription<T>;
    

        Task RemoveEntityByIdAsync<T>(string id)
          where T : IEntity;
    
    }
}
