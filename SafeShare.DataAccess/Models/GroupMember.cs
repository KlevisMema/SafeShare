namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a member of a group.
/// </summary>
public class GroupMember
{
    /// <summary>
    /// Gets or sets the unique identifier of the group member.
    /// </summary>
    public int GroupMemberId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated group.
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the member.
    /// </summary>
    public string MemberUserId { get; set; }

    /// <summary>
    /// Gets or sets the balance specific to the group for the member.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Navigation property representing the group to which the member belongs.
    /// </summary>
    public Group Group { get; set; }

    /// <summary>
    /// Navigation property representing the member.
    /// </summary>
    public ApplicationUser Member { get; set; }

    /// <summary>
    /// Collection of expenses paid by the member.
    /// </summary>
    public ICollection<Expense> PaidExpenses { get; set; }

    /// <summary>
    /// Collection of expenses owed by the member.
    /// </summary>
    public ICollection<Expense> OwedExpenses { get; set; }
}