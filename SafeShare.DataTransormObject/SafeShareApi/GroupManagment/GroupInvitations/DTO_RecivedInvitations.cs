/* 
 * This file defines the DTO_RecivedInvitations class in the SafeShare.DataTransormObject.GroupManagment.GroupInvitations namespace.
 * The DTO_RecivedInvitations class is a Data Transfer Object used to encapsulate the details of received group invitations,
 * including the invitation message, details about the inviting user and the group involved.
 */

using SafeShare.Utilities.SafeShareApi.Enums;

namespace SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

/// <summary>
/// Represents the details of a received group invitation.
/// This class includes information such as the invitation message, details of the inviting user, and the group to which the invitation pertains.
/// </summary>
public class DTO_RecivedInvitations
{
    /// <summary>
    /// Gets or sets the message included in the invitation.
    /// </summary>
    public string InvitationMessage { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the unique identifier of the user who sent the invitation.
    /// </summary>
    public Guid InvitingUserId { get; set; }
    /// <summary>
    /// Gets or sets the name of the user who sent the invitation.
    /// </summary>
    public string InvitingUserName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the unique identifier of the group to which the invitation is related.
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the name of the group to which the invitation is related.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the InvitationStatus of the group to which the invitation is related.
    /// </summary>
    public InvitationStatus InvitationStatus { get; set; }
    /// <summary>
    /// Gets or sets the InvitationId of the group.
    /// </summary>
    public Guid InvitationId { get; set; }
}