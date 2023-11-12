/* 
 * This file defines the DTO_GroupType class in the SafeShare.DataTransormObject.GroupManagment namespace.
 * The DTO_GroupType class is a Data Transfer Object used to encapsulate basic information about a group,
 * including its unique identifier and name.
 */

namespace SafeShare.DataTransormObject.GroupManagment;

/// <summary>
/// Represents basic information about a group, including its unique identifier and name.
/// This class is typically used to convey group information in lists or summaries.
/// </summary>
public class DTO_GroupType
{
    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    public Guid GroupId { get; set; }
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
}