/*
 * Defines the GroupMember class that represents a member of a group.
 * This file contains definitions for the GroupMember's properties, their annotations, and relationships.
*/

using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeShare.DataAccessLayer.Models.SafeShareApi;

/// <summary>
/// Represents a member of a group.
/// </summary>
public class GroupMember : Base
{
    /// <summary>
    /// Gets or sets the balance specific to the group for the member.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Balance { get; set; }
    /// <summary>
    /// Gets or sets a flag indicating whether this member is the owner of the group.
    /// </summary>
    [Required]
    public bool IsOwner { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the associated group.
    /// </summary>
    [Required]
    public Guid GroupId { get; set; }
    /// <summary>
    /// Navigation property representing the group to which the member belongs. 
    /// </summary>
    public virtual Group Group { get; set; } = null!;
    /// <summary>
    /// Gets or sets the unique identifier of the associated application user.
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!;
    /// <summary>
    /// Navigation property representing the application user associated with the member.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}