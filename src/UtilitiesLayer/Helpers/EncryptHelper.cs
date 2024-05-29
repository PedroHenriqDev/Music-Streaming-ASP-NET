using DomainLayer.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace UtilitiesLayer.Helpers
{
    public static class EncryptHelper
    {
        public static string GenerateEncryptedString()
        {
            string sessionId = Guid.NewGuid().ToString();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] sessionIdBytes = Encoding.UTF8.GetBytes(sessionId);
                byte[] hashedBytes = sha256.ComputeHash(sessionIdBytes);
                string encryptedSessionId = BitConverter.ToString(hashedBytes).Replace("-", "");
                return encryptedSessionId;
            }
        }

        public static string EncryptPasswordSHA512(string password)
        {
            if (password is null)
            {
                throw new EncryptException("Error encrypting password");
            }

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < Math.Min(hashBytes.Length, 50 / 2); i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static string GetFileHash(byte[] data)
        {
            if (data is null)
            {
                throw new EncryptException("A problem occurred when trying to convert data to hash, due to null reference reasons!");
            }

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(data);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
