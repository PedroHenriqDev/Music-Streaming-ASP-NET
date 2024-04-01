using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IEntityWithDescription<T> : IEntityWithName<T>
    {
        public string Description { get; set; }
    }
}
