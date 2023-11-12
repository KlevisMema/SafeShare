/* 
 * This file defines the DTO_CreateGroup class in the SafeShare.DataTransormObject.GroupManagment namespace.
 * The DTO_CreateGroup class is a Data Transfer Object used to encapsulate the information required to create a new group,
 * including the group's name and description.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.GroupManagment;

/// <summary>
/// Represents the data required to create a new group.
/// This class includes properties for the group's name and description.
/// </summary>
public class DTO_CreateGroup
{
    /// <summary>
    /// Gets or sets the name of the group.
    /// This property is required and has a maximum length of 100 characters.
    /// </summary>
    [Required(ErrorMessage = "Group name is required"), StringLength(100)]
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the description of the group.
    /// This property is required and has a maximum length of 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Group description is required"), StringLength(200)]
    public string GroupDescription { get; set; } = string.Empty;
}