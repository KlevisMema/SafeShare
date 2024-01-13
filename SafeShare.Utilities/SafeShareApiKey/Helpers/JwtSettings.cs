/* 
 * Contains helper classes for managing API key and JWT settings in the SafeShare application.
 * 
 * 'It is the same as the other model defined in ../SafeShareApiKey.ConfigurationSettings but its created in its
 * own folder beacuse in the future the configuration might change from the 2 different places its used.'
 */

namespace SafeShare.Utilities.SafeShareApiKey.Helpers;

/// <summary>
/// Represents the settings for JSON Web Token (JWT) authentication.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Configuration section name for JWT settings.
    /// </summary>
    public const string SectionName = "Jwt";
    /// <summary>
    /// The intended audience for the token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    /// <summary>
    /// The secret key for generating the token.
    /// </summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>
    /// The issuer of the token.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    /// <summary>
    /// The lifetime of the token in minutes.
    /// </summary>
    public int LifeTime { get; set; } = 0;
}