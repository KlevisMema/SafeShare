/* 
 * This file defines the DTO_SendInvitationRequest class in the SafeShare.DataTransormObject.GroupManagment.GroupInvitations namespace.
 * The DTO_SendInvitationRequest class is a Data Transfer Object used to encapsulate the necessary information for sending a group invitation,
 * including the identifiers of the inviting and invited users, the group involved, and the invitation message.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

/// <summary>
/// Represents the information required to send a group invitation.
/// This class includes properties for the identifiers of the inviting and invited users, the group ID, and the invitation message.
/// </summary>
public class DTO_SendInvitationRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the user sending the invitation.
    /// </summary>
    [Required]
    public Guid InvitingUserId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user being invited.
    /// </summary>
    [Required]
    public Guid InvitedUserId { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the group to which the invitation pertains.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the invitation message.
    /// </summary>
    [Required]
    public string InvitaitonMessage { get; set; } = string.Empty;
}