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
    public virtual ICollection<Group>? Groups { get; set; }// Nullable since the user might not own any groups initially.

    /// <summary>
    /// Navigation property for the expenses created by the user.
    /// </summary>
    public virtual ICollection<Expense>? Expenses { get; set; } // Nullable since the user might not be a member of any groups initially.
}
