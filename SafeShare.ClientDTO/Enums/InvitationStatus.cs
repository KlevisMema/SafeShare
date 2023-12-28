using System.Text.Json.Serialization;

namespace SafeShare.ClientDTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvitationStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3
}