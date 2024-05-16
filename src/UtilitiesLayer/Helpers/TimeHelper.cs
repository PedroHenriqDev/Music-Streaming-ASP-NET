namespace UtilitiesLayer.Helpers
{
    static public class TimeHelper
    {
        public static bool HasElapsedSinceLastView(DateTime lastViewTime) 
        {
            TimeSpan timeElapsed = DateTime.UtcNow - lastViewTime;
            return timeElapsed.Minutes > 2;
        }
    }
}
