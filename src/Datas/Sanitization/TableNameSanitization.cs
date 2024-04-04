using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Datas.Sanitization
{
    public class TableNameSanitization
    {
        public static string GetPluralTableName<T>()
        {
            return typeof(T).Name + "s";
        }

        public static string GetAssociationTableGenre<T>() 
        {
            return typeof(T).Name + "Genre";
        }
    }
}
