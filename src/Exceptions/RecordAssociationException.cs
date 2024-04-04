using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public class RecordAssociationException : Exception
    {
        public RecordAssociationException(string message) : base(message)
        {
        }
    }
}
