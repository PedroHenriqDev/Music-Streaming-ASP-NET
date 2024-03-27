namespace Extensions
{
    static public class StringExtension
    {
        static public string CutName(this string str)
        {
            if (str == null)
            {
                return str;
            }

            return str.Split(' ')[0];
        }
    }
}
