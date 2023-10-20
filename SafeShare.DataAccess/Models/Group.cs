namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a group of users for encoding common expenses.
/// </summary>
public class Group
{
    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string GroupName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the group owner (user who created the group).
    /// </summary>
    public string OwnerUserId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property representing the owner of the group.
    /// </summary>
    public ApplicationUser Owner { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of members in the group.
    /// </summary>
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

    /// <summary>
    /// Gets or sets the collection of expenses associated with the group.
    /// </summary>
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}