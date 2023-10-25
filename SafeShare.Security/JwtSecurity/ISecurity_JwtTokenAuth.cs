/*
    This file contains the interface IOAuthJwtTokenService, which defines the contract for creating JWT tokens for authentication.
*/

using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Security.JwtSecurity;

/// <summary>
/// Interface for creating JWT tokens for authentication.
/// </summary>
public interface ISecurity_JwtTokenAuth
{
    /// <summary>
    /// Creates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user information. <see cref="UserDto"/></param>
    /// <returns>The generated JWT token.</returns>
    string
    CreateToken
    (
        DTO_AuthUser user
    );
}