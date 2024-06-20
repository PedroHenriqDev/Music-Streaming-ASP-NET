namespace UtilitiesLayer.Extensions;

static public class StringExtension
{
    static public string CutName(this string str)
    {
        if (str is null)
        {
            throw new ArgumentNullException("Error in cut name, because reference null");
        }

        return str.Split(' ')[0];
    }
    
    static public List<string> ConvertStringJoinInList(this string str, string separator = ",") 
    {
        if(str is null)
        {
            throw new ArgumentNullException("Error in convert string join in List of string, because reference null");
        }

        return new List<string>(str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries));
    }
}
