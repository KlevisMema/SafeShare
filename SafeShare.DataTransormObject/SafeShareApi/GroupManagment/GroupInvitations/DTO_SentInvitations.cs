/* 
 * This file defines the DTO_SentInvitations class in the SafeShare.DataTransormObject.GroupManagment.GroupInvitations namespace.
 * The DTO_SentInvitations class is a Data Transfer Object used to encapsulate details of sent group invitations,
 * including information about the invited user, the group, and the status and time of the invitation.
 */

using SafeShare.Utilities.SafeShareApi.Enums;

namespace SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

/// <summary>
/// Represents the details of a sent group invitation.
/// This class includes information about the recipient of the invitation, the group, the time when the invitation was sent,
/// the current status of the invitation, and the unique identifiers of the invitation and the invited user.
/// </summary>
public class DTO_SentInvitations
{
    /// <summary>
    /// Gets or sets the name of the user who was invited.
    /// </summary>
    public string User { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the group to which the invitation pertains.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the date and time when the invitation was sent.
    /// </summary>
    public DateTime InvitationTimeSend { get; set; }
    /// <summary>
    /// Gets or sets the status of the invitation, such as pending, accepted, or rejected.
    /// </summary>
    public InvitationStatus InvitationStatus { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the invitation.
    /// </summary>
    public Guid InvitationId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user who was invited.
    /// </summary>
    public Guid InvitedUserId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the group to which the invitation was sent.
    /// </summary>
    public Guid GroupId { get; set; }
}