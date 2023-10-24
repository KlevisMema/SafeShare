using SafeShare.DataAccessLayer.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a group of users for encoding common expenses.
/// </summary>
public class Group : Base
{
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [Required, StringLength(100)]
    public string Name { get; set; } = null!; // Non-nullable since a group must have a name.

    /// <summary>
    /// Navigation property for the users belonging to this group.
    /// </summary>
    public virtual ICollection<GroupMember> Users { get; set; } = null!;

    /// <summary>
    /// Navigation property for the expenses associated with this group.
    /// </summary>
    public virtual ICollection<Expense>? Expenses { get; set; }
}