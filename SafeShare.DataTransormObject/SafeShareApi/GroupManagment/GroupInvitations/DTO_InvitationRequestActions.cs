/* 
 * This file defines the DTO_InvitationRequestActions class in the SafeShare.DataTransormObject.GroupManagment.GroupInvitations namespace.
 * The DTO_InvitationRequestActions class is a Data Transfer Object used to encapsulate the necessary details for actions related to group invitations.
 * It includes the identifiers for the inviting user, the group, the invitation itself, and the invited user.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

/// <summary>
/// Represents the necessary details for performing actions related to group invitations.
/// This class includes the unique identifiers of the inviting user, the group, the specific invitation, and the invited user.
/// </summary>
public class DTO_InvitationRequestActions
{
    /// <summary>
    /// Gets or sets the unique identifier of the user sending the invitation.
    /// </summary>
    [Required]
    public Guid InvitingUserId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the group to which the invitation pertains.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the invitation.
    /// </summary>
    [Required]
    public Guid InvitationId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user being invited.
    /// </summary>
    [Required]
    public Guid InvitedUserId { get; set; }
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the user who accepted the invitation.
    /// </summary>
    public string UserWhoAcceptedTheInvitation { get; set; } = string.Empty;
}