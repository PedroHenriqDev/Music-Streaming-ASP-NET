using MusicWeave.Datas;
using MusicWeave.Exceptions;
using System.Text;
using System.Security.Cryptography;

namespace MusicWeave.Models.Services
{
    public class EncryptService
    {

        private readonly ILogger<EncryptService> _logger;

        public EncryptService(ILogger<EncryptService> logger)
        {
            _logger = logger;
        }

        public string EncryptPasswordSHA512(string password) 
        {
            if (password == null) 
            {
                _logger.LogCritical("Brutal error when encrypting error");
                throw new EncryptException("Error encrypting password");
            }

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hasgBytes = sha512.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < Math.Min(hasgBytes.Length, 50 / 2); i++)
                {
                    builder.Append(hasgBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
