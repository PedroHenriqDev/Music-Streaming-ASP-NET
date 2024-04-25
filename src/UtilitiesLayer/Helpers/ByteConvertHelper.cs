using Microsoft.AspNetCore.Http;

namespace UtilitiesLayer.Helpers
{
    static public class ByteConvertHelper
    {
        static public async Task<byte[]> ConvertIFormFileInByteAsync(IFormFile formFile) 
        {
            using(var memoryStream = new MemoryStream()) 
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
