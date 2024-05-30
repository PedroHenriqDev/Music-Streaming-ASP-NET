namespace UtilitiesLayer.Helpers
{
    public class CookiesAndSessionsKeys
    {
        public static string UserSessionKey { get; } = EncryptHelper.GenerateEncryptedString();
        public static string PlaylistSessionKey { get; } = EncryptHelper.GenerateEncryptedString();
        public static string PlaylistIdSessionKey { get; } = EncryptHelper.GenerateEncryptedString();
        public static string UserIdClaimKey { get; } = EncryptHelper.GenerateEncryptedString(); 
    }
}
