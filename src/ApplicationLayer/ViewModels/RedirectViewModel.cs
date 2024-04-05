using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.ViewModels
{
    public class RedirectViewModel<T> 
    {
        public string RedirectUrl { get; set; }
        public T? Value { get; set; }
    }
}
