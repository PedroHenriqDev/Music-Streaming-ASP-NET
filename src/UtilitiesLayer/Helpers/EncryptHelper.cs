using System.Security.Cryptography;
using System.Text;

namespace UtilitiesLayer.Helpers
{
    static public class EncryptHelper
    {
        static public string GenerateEncryptedString()
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
    }
}
