using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Helpers
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
