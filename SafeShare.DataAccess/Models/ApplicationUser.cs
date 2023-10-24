using SafeShare.DataAccessLayer.BaseModels;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a registered user in the system.
/// Inherits from IdentityUser with Guid as the primary key.
/// </summary>
public class ApplicationUser : BaseIdentity
{
    /// <summary>
    /// Navigation property representing the groups owned by the user.
    /// </summary>
    public virtual ICollection<GroupMember>? GroupMembers { get; set; }

    /// <summary>
    /// Navigation property for the relationship of the user as a member in various expenses.
    /// </summary>
    public virtual ICollection<ExpenseMember>? ExpenseMembers { get; set; }
}
