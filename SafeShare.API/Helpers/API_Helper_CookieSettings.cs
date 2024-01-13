/* 
 * Contains helper classes and utilities for the SafeShare API.
 */

using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Utilities.SafeShareApiKey.Helpers;

namespace SafeShare.API.Helpers;

/// <summary>
/// Represents the settings for cookies used in the application, typically loaded from a configuration section.
/// </summary>
public class API_Helper_CookieSettings
{
    /// <summary>
    /// Configuration section name for cookie settings.
    /// </summary>
    public const string SectionName = "CookieSettings";
    /// <summary>
    /// Name of the cookie used for authentication tokens.
    /// </summary>
    public string AuthTokenCookieName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the cookie used for refresh authentication tokens.
    /// </summary>
    public string RefreshAuthTokenCookieName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the cookie used for storing the refresh authentication token identifier.
    /// </summary>
    public string RefreshAuthTokenIdCookieName { get; set; } = string.Empty;
}