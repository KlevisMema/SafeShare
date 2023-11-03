/*
    This file contains the interface IOAuthJwtTokenService, which defines the contract for creating JWT tokens for authentication.
*/

using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Security.JwtSecurity;

/// <summary>
/// Interface for creating JWT tokens for authentication.
/// </summary>
public interface ISecurity_JwtTokenAuth<TService, TFuncInputParamType>
{
    /// <summary>
    /// Creates a JWT token for the specified user.
    /// </summary>
    /// <param name="TFuncInputParamType">The user information. <see cref="TFuncInputParamType"/></param>
    /// <returns>The generated JWT token.</returns>
    string
    CreateToken
    (
        TFuncInputParamType user
    );
}