/*
 * Defines the Group class that represents a group of users for encoding common expenses.
 * This file contains definitions for the Group's properties, their annotations, and relationships.
*/

using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models.SafeShareApi;

/// <summary>
/// Represents a group of users for encoding common expenses.
/// </summary>
public class Group : Base
{
    /// <summary>
    /// Gets or sets the primary key of the group
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the tak of the group
    /// </summary>
    public Guid Tag {  get; set; }
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [Required, StringLength(100)]
    public string Name { get; set; } = null!;
    /// <summary>
    /// Gets or sets the description of the group.
    /// </summary>
    [Required, StringLength(200)]
    public string Description { get; set; } = null!;
    /// <summary>
    /// Navigation property for the users belonging to this group.
    /// </summary>
    public virtual ICollection<GroupMember> GroupMembers { get; set; } = null!;
    /// <summary>
    /// Navigation property for the expenses associated with this group.
    /// </summary>
    public virtual ICollection<Expense>? Expenses { get; set; }

    public virtual ICollection<GroupInvitation>? Invitations { get; set; }
}