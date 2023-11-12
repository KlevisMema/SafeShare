/* 
 * This file defines the DTO_EditGroup class in the SafeShare.DataTransormObject.GroupManagment namespace.
 * The DTO_EditGroup class is a Data Transfer Object used for encapsulating the data required to edit an existing group,
 * including the updated group name and description.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.GroupManagment;

/// <summary>
/// Represents the data required to edit an existing group.
/// This class includes properties for the updated group name and description.
/// </summary>
public class DTO_EditGroup
{
    /// <summary>
    /// Gets or sets the updated name for the group.
    /// This property is required and has a maximum length of 100 characters.
    /// </summary>
    [Required(ErrorMessage = "Group name is required"), StringLength(100)]
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the updated description for the group.
    /// This property is required and has a maximum length of 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Group description is required"), StringLength(200)]
    public string GroupDescription { get; set; } = string.Empty;
}