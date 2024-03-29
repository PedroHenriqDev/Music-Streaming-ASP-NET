using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    static public class DateTimeExtension
    {
        static public bool HasValue(this DateTime date) 
        {
            if(date == null) 
            {
                return false;
            }

            var dateTime = DateTime.MinValue;
            return date > DateTime.MinValue; 
        }
    }
}
