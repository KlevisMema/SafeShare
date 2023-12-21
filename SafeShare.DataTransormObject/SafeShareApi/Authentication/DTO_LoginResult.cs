/* 
 * This file defines the DTO_LoginResult class in the SafeShare.DataTransormObject.Authentication namespace.
 * The DTO_LoginResult class is a Data Transfer Object used to encapsulate the result of a user login attempt.
 * It includes the authentication token, a flag indicating whether an OTP is required for login, and the user's ID.
 */

using SafeShare.DataTransormObject.SafeShareApi.Security;

namespace SafeShare.DataTransormObject.SafeShareApi.Authentication;

/// <summary>
/// Represents the result of a user login attempt, including the authentication token,
/// a flag for OTP requirement, and the user's unique identifier.
/// </summary>
public class DTO_LoginResult
{
    /// <summary>
    /// Gets or sets the authentication token provided upon successful login.
    /// </summary>
    public DTO_Token? Token { get; set; } = new();
    /// <summary>
    /// Gets or sets a value indicating whether an OTP (One Time Password) is required during the login process.
    /// </summary>
    public bool RequireOtpDuringLogin { get; set; } = false;
    /// <summary>
    /// Gets or sets the unique identifier of the user who has logged in.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}