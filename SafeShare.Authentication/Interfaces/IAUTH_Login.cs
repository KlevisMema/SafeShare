/*
     * This interface provides the signature for the login 
     * functionality within the Authentication module. It ensures 
     * that any implementing class provides a method for authenticating 
     * and logging in users based on the provided DTO.
*/

using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.Authentication;

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
    Task<Util_GenericResponse<string>>
    LoginUser
    (
        DTO_Login loginDto
    );
}