using DomainLayer.Exceptions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ApplicationLayer.Services
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

        public string GetFileHash(byte[] data)
        {
            if (data == null)
            {
                _logger.LogError("A problem occurred when trying to convert data to hash, due to null reference reasons");
                throw new EncryptException("A problem occurred when trying to convert data to hash, due to null reference reasons!");
            }

            using (SHA512 sha5112 = SHA512.Create())
            {
                byte[] hashBytes = sha5112.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
