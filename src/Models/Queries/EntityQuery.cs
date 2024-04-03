using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Queries
{
    public class EntityQuery<T>
    {
        public bool Result { get; set; }
        public string Message {  get; set; }
        public T Entity { get; set; }
        public DateTime Moment {  get; set; }

        public EntityQuery() 
        {
        }

        public EntityQuery(bool result, string message, T entity, DateTime moment)
        {
            Result = result;
            Message = message;
            Entity = entity;
            Moment = moment;
        }
    }
}
