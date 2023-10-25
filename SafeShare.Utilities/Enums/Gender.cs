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