/* 
 * This file defines the DTO_GroupsTypes class in the SafeShare.DataTransormObject.GroupManagment namespace.
 * The DTO_GroupsTypes class is a Data Transfer Object used to encapsulate information about the types of groups a user is involved with,
 * specifically, groups that the user has created and groups that the user has joined.
 */

namespace SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

/// <summary>
/// Represents the types of groups a user is involved with, categorizing them into groups created and groups joined.
/// </summary>
public class DTO_GroupsTypes
{
    /// <summary>
    /// Gets or sets the list of groups created by the user.
    /// This property may be null if the user has not created any groups.
    /// </summary>
    public List<DTO_GroupType>? GroupsCreated { get; set; }
    /// <summary>
    /// Gets or sets the list of groups that the user has joined.
    /// This property may be null if the user has not joined any groups.
    /// </summary>
    public List<DTO_GroupType>? GroupsJoined { get; set; }
    /// <summary>
    /// Gets or sets the list of all groups that the user has joined and created.
    /// </summary>
    public List<DTO_GroupDetails>? AllGroupsDetails { get; set; }
}