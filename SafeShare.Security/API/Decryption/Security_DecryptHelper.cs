using System.Text;
using System.Security.Cryptography;

namespace SafeShare.Security.API.Decryption;

internal class Security_DecryptHelper
{
    internal static string?
    DecryptWithPrivateKey
    (
        string base64Input
    )
    {
        var privateKey = Environment.GetEnvironmentVariable("SAFE_SHARE_API_PRIVATE_KEY");

        if (privateKey == null)
            return null;

        byte[] bytesToDecrypt = Convert.FromBase64String(base64Input);
        using var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(privateKey);
        byte[] decryptedBytes = rsa.Decrypt(bytesToDecrypt, true);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}