/* 
 * Contains helper classes and methods for handling API key generation and hashing within the SafeShare application.
 */

using System.Text;
using System.Security.Cryptography;

namespace SafeShare.Utilities.SafeShareApiKey.Helpers;

/// <summary>
/// Provides methods for generating and hashing API keys.
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Generates a new API key based on a user ID.
    /// </summary>
    /// <param name="userId">The user ID to base the API key on.</param>
    /// <returns>A newly generated API key.</returns>
    public static string
    GenerateApiKey
    (
        string userId
    )
    {
        var keySize = 32;
        var randomBytes = new byte[keySize];
        RandomNumberGenerator.Fill(randomBytes);

        var userIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId));
        var apiKeyPart = Convert.ToBase64String(randomBytes);

        return userIdBase64 + apiKeyPart;
    }
    /// <summary>
    /// Hashes an API key using SHA256.
    /// </summary>
    /// <param name="apiKey">The API key to hash.</param>
    /// <returns>The hashed representation of the API key.</returns>
    public static string
    HashApiKey
    (
        string apiKey
    )
    {
        var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);
        var hashedBytes = SHA256.HashData(apiKeyBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
    }
}