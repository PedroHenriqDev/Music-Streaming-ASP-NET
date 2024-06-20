using Microsoft.AspNetCore.Http;
using NAudio.Wave;

namespace UtilitiesLayer.Helpers;

static public class MusicHelper
{
    static public TimeSpan GetDuration(IFormFile file) 
    {
        using (var stream = file.OpenReadStream())
        {
            var audioFile = new Mp3FileReader(stream);
            return audioFile.TotalTime;
        } 
    }

    static public string FormatMusicDuration(this TimeSpan duration)
    {
        int minutes = (int)duration.TotalMinutes;
        int seconds = duration.Seconds;

        return $"{minutes}:{seconds:D2}";
    }
}
