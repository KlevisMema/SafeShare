using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a member of a group.
/// </summary>
public class GroupMember : Base
{
    /// <summary>
    /// Gets or sets the unique identifier of the associated group.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; } // Non-nullable foreign key.

    /// <summary>
    /// Gets or sets the unique identifier of the associated application user.
    /// </summary>
    [Required]
    public string ApplicationUserId { get; set; } = null!; // Non-nullable since a member must have a user ID.

    /// <summary>
    /// Gets or sets the balance specific to the group for the member.
    /// </summary>
    [Required]
    public decimal Balance { get; set; } // Non-nullable since balance must have a default value.

    /// <summary>
    /// Gets or sets a flag indicating whether this member is the owner of the group.
    /// </summary>
    [Required]
    public bool IsOwner { get; set; } // Indicates if the member is the owner of the group.

    /// <summary>
    /// Navigation property representing the group to which the member belongs. 
    /// </summary>
    public virtual Group Group { get; set; } = null!; // Non-nullable since a member must belong to a group.

    /// <summary>
    /// Navigation property representing the application user associated with the member.
    /// </summary>
    public virtual ApplicationUser ApplicationUser { get; set; } = null!; // Non-nullable since a member must be associated with a user.
}