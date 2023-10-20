using Microsoft.AspNetCore.Identity;

namespace SafeShare.DataAccessLayer.Models;

/// <summary>
/// Represents a user of the application.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    public string FullName { get; set; } = null!; // Non-nullable since a full name is usually required.

    /// <summary>
    /// Navigation property representing the groups owned by the user.
    /// </summary>
    public virtual ICollection<Group>? OwnedGroups { get; set; }// Nullable since the user might not own any groups initially.

    /// <summary>
    /// Navigation property representing the groups in which the user is a member.
    /// </summary>
    public virtual ICollection<GroupMember>? MemberOfGroups { get; set; } // Nullable since the user might not be a member of any groups initially.
}
