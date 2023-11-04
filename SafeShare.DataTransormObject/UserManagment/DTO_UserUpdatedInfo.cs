/* 
 * Defines a data transfer object for updated user information.
 */

using SafeShare.DataTransormObject.Security;
using SafeShare.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.UserManagment;

/// <summary>
/// Represents a data transfer object for user's updated information.
/// </summary>
public class DTO_UserUpdatedInfo
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public string UserID { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the age of the user.
    /// </summary>
    public int Age { get; set; }
    /// <summary>
    /// Gets or sets the birthday of the user.
    /// </summary>
    public DateTime Birthday { get; set; }
    /// <summary>
    /// Gets or sets the gender of the user.
    /// </summary>
    public Gender Gender { get; set; }
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    public DTO_Token? UserToken { get; set; }
}