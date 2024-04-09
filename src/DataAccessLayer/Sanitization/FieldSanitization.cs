using System.Text.RegularExpressions;

namespace DataAccessLayer.Sanitization
{
    public static class FieldSanitization
    {

        public static string SanitizeId(string id) 
        {
            return Regex.Replace(id, @"[^\w-]", "");
        }

        public static string JoinIds(IEnumerable<string> ids)
        {
            var sanitizedIds = ids.Select(SanitizeId);
            return string.Join(",", sanitizedIds.Select(id => $"'{id}'"));
        }

        public static string ForeignKeyName(string name) 
        {
            return $"{name}Id";
        }
    }
}
