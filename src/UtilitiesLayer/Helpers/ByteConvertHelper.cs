using Microsoft.AspNetCore.Http;

namespace UtilitiesLayer.Helpers
{
    public class ByteConvertHelper
    {
        public async Task<byte[]> ConvertIFormFileInByte(IFormFile formFile) 
        {
            using(var memoryStream = new MemoryStream()) 
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
