namespace UtilitiesLayer.Extensions
{
    static public class StringExtension
    {
        static public string CutName(this string str)
        {
            if (str is null)
            {
                return str;
            }

            return str.Split(' ')[0];
        }
    }
}
