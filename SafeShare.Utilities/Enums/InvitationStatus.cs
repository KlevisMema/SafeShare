/* 
 * Defines enumeration for invitation statuses within the application.
 * This enumeration is used to represent the different states of an invitation, 
 * such as pending, accepted, or rejected.
 */

using System.Text.Json.Serialization;

namespace SafeShare.Utilities.Enums;

/// <summary>
/// Represents the status of an invitation in the application.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvitationStatus
{
    /// <summary>
    /// The invitation has been sent and is awaiting response. Value: 1.
    /// </summary>
    Pending = 1,
    /// <summary>
    /// The invitation has been accepted. Value: 2.
    /// </summary>
    Accepted = 2,
    /// <summary>
    /// The invitation has been rejected. Value: 3.
    /// </summary>
    Rejected = 3
}