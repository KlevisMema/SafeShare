/* 
 * Provides cryptographic decryption functionalities within the application.
 * This class includes methods to decrypt data using private keys.
 */

using System.Text;
using System.Security.Cryptography;

namespace SafeShare.Security.API.Decryption;

/// <summary>
/// Provides helper methods for cryptographic decryption.
/// </summary>
internal class Security_DecryptHelper
{
    /// <summary>
    /// Decrypts the given input string using a private key.
    /// </summary>
    /// <param name="base64Input">The encrypted data in base64 format.</param>
    /// <returns>The decrypted string if successful; null if the private key is not found or the decryption fails.</returns>
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