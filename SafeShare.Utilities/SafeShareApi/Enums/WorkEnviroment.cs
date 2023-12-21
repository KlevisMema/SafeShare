/* 
 * Contains enumerations used throughout the SafeShare API for defining various constants and types.
 */

using System.Text.Json.Serialization;

namespace SafeShare.Utilities.SafeShareApi.Enums;

/// <summary>
/// Represents the different work environments in which the application can operate.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkEnvironment
{
    /// <summary>
    /// The testing environment, used for testing purposes.
    /// </summary>
    Testing = 0,
    /// <summary>
    /// The development environment, used during the development phase.
    /// </summary>
    Development = 1,
    /// <summary>
    /// The production environment, used for the live, deployed application.
    /// </summary>
    Production = 2,
}