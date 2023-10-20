using SafeShare.Utilities.Enums;
using Microsoft.AspNetCore.Identity;
using SafeShare.DataAccessLayer.BaseModels;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a user of the application.
/// </summary>
public class ApplicationUser : BaseIdentity
{
    /// <summary>
    /// Navigation property representing the groups owned by the user.
    /// </summary>
    public virtual ICollection<Group>? OwnedGroups { get; set; }// Nullable since the user might not own any groups initially.

    /// <summary>
    /// Navigation property representing the groups in which the user is a member.
    /// </summary>
    public virtual ICollection<GroupMember>? MemberOfGroups { get; set; } // Nullable since the user might not be a member of any groups initially.
}
