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
    public string? FullName { get; set; }

    /// <summary>
    /// Navigation property representing the groups owned by the user.
    /// </summary>
    public virtual ICollection<Group> OwnedGroups { get; set; } = new List<Group>();

    /// <summary>
    /// Navigation property representing the groups in which the user is a member.
    /// </summary>
    public virtual ICollection<GroupMember> MemberOfGroups { get; set; } = new List<GroupMember>();
}
