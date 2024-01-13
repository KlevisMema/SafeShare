/*
 * This file defines the GroupInvitation class in the SafeShare.DataAccessLayer.Models namespace.
 * The GroupInvitation class represents an invitation to join a group.
 */

using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SafeShare.Utilities.SafeShareApi.Enums;

namespace SafeShare.DataAccessLayer.Models.SafeShareApi;

public class GroupInvitation : Base
{
    /// <summary>
    /// Gets or sets the unique identifier for the group invitation.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the invitation message sent with the group invitation.
    /// </summary>
    [Required, StringLength(200)]
    public string InvitationMessage { get; set; } = null!;
    /// <summary>
    /// Gets or sets the status of the group invitation (e.g., pending, accepted, declined).
    /// </summary>
    [Required]
    public InvitationStatus InvitationStatus { get; set; }
    /// <summary>
    /// Gets or sets the ID of the group associated with the invitation.
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the group associated with the invitation.
    /// </summary>
    public virtual Group Group { get; set; } = null!;
    /// <summary>
    /// Gets or sets the ID of the user who received the invitation.
    /// </summary>
    public string InvitedUserId { get; set; } = null!;
    /// <summary>
    /// Gets or sets the user who received the invitation.
    /// </summary>
    public virtual ApplicationUser InvitedUser { get; set; } = null!;
    /// <summary>
    /// Gets or sets the ID of the user who sent the invitation.
    /// </summary>
    public string InvitingUserId { get; set; } = null!;
    /// <summary>
    /// Gets or sets the user who sent the invitation.
    /// </summary>
    public virtual ApplicationUser InvitingUser { get; set; } = null!;
}