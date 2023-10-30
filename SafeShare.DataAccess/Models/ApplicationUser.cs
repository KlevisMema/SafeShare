/*
* Defines the ApplicationUser class that represents registered users within the system.
* This file contains definitions for the ApplicationUser's properties and their relationships.
*/

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

    public virtual ICollection<GroupInvitation>? SentInvitations { get; set; }
    public virtual ICollection<GroupInvitation>? ReceivedInvitations { get; set; }
}
