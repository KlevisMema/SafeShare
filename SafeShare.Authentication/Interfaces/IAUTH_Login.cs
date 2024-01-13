/*
     * This interface provides the signature for the login 
     * functionality within the Authentication module. It ensures 
     * that any implementing class provides a method for authenticating 
     * and logging in users based on the provided DTO.
*/

using SafeShare.DataTransormObject.SafeShareApi.Authentication;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.Authentication.Interfaces;

/// <summary>
/// Defines the contract for the login operations within the Authentication module.
/// </summary>
public interface IAUTH_Login
{
    /// <summary>
    /// Authenticates and logs in a user based on the provided login data transfer object.
    /// </summary>
    /// <param name="loginDto">The data transfer object containing user login details.</param>
    /// <returns>A generic response with a JWT token (if successful) or an error message.</returns>
    Task<Util_GenericResponse<DTO_LoginResult>>
    LoginUser
    (
        DTO_Login loginDto
    );
    /// <summary>
    /// Confirm the login of the user
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="otp">The one time password </param>
    /// <returns>A generic respone with jwt token or a error message</returns>
    Task<Util_GenericResponse<DTO_LoginResult>>
    ConfirmLogin
    (
        Guid userId,
        string otp
    );
    /// <summary>
    /// Log out a user
    /// </summary>
    /// <returns> Asyncronous Task</returns>
    Task
    LogOut
    (
        string userId
    );
}