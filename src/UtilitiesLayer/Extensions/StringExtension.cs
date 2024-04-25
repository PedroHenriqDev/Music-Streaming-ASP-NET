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

        static public string FormatDuration(this TimeSpan duration) 
        {
            int minutes = (int)duration.TotalMinutes;
            int seconds = duration.Seconds;

            return $"{minutes}:{seconds:D2}";
        }
    }
}
