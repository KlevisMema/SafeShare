/* 
 * This file defines the DTO_GroupDetails class in the SafeShare.DataTransormObject.GroupManagment namespace.
 * The DTO_GroupDetails class is a Data Transfer Object used to encapsulate detailed information about a group,
 * including its name, number of members, latest expenses, description, the admin's details, creation date, and total expenditure.
 */

namespace SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

/// <summary>
/// Represents detailed information about a group.
/// This class includes properties for the group's name, number of members, recent expenses, description,
/// administrator details, creation date, and total expenditure.
/// </summary>
public class DTO_GroupDetails
{
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the number of members in the group.
    /// </summary>
    public int NumberOfMembers { get; set; }
    /// <summary>
    /// Gets or sets the details of the latest expense recorded in the group.
    /// </summary>
    public string LatestExpense { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the description of the group.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the group's administrator.
    /// </summary>
    public string GroupAdmin { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the date when the group was created.
    /// </summary>
    public DateTime GroupCreationDate { get; set; }
    /// <summary>
    /// Gets or sets the total amount spent by the group.
    /// </summary>
    public decimal TotalSpent { get; set; }
    /// <summary>
    /// Gets or sets the users when of the group.
    /// </summary>
    public List<DTO_UsersGroupDetails>? UsersGroups { get; set; }
}