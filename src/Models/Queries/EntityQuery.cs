using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Queries
{
    public class EntityQuery<T> where T : class, IEntity
    {
        public bool Result { get; set; }
        public string Message {  get; set; }
        public T Entity { get; set; }
    }
}
