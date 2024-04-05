namespace DataAccessLayer.Sanitization
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
