/* 
 * This file defines the DTO_ActivateAccount class in the SafeShare.DataTransormObject.UserManagment namespace.
 * The DTO_ActivateAccount class is a Data Transfer Object used to encapsulate the necessary information for activating a user account,
 * including the user's email, username, and password.
 */

using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApi.UserManagment;

/// <summary>
/// Represents the information required to activate a user account.
/// This class includes the user's email, username, and password.
/// </summary>
public class DTO_ActivateAccount
{
    /// <summary>
    /// Gets or sets the email address associated with the user account. 
    /// This property is required and should be a valid email address.
    /// </summary>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the username for the user account. 
    /// This property is required and has a maximum length of 100 characters.
    /// </summary>
    [Required, StringLength(100)]
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the password for the user account. 
    /// This property is required and should follow the data type format for passwords.
    /// </summary>
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}