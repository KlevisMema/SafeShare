/*
 * Implementation of the ISecurity_JwtTokenAuth interface responsible for JWT token generation.
 * This service provides methods to create JWT tokens for user authentication, leveraging provided JWT settings.
*/

namespace SafeShare.Security.JwtSecurity.Interfaces;

public interface ISecurity_JwtTokenHash
{
    /// <summary>
    /// Validates a jwt token
    /// </summary>
    /// <param name="tokenToValidate">The token to validate</param>
    /// <returns>True or false</returns>
    Task<bool>
    ValidateToken
    (
       string tokenToValidate,
       string hashedTokenInDb
    );
}