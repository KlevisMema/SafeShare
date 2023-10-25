/*
 * Enumerates the different genders for a user.
 * Supports JSON serialization using a string representation of the enum values.
*/

using System.Text.Json.Serialization;

namespace SafeShare.Utilities.Enums;

/// <summary>
/// An enum that represents gender for the user
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    Male = 1,
    Female = 2
}