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
    public string GroupName { get; set; } = null!; // Non-nullable since a group must have a name.

    /// <summary>
    /// Navigation property representing the members of the group.
    /// </summary>
    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>(); // Non-nullable collection to ensure initialization.
}