/*
 * Implementation of the ISecurity_JwtTokenAuth interface responsible for JWT token generation.
 * This service provides methods to create JWT tokens for user authentication, leveraging provided JWT settings.
*/

namespace SafeShare.Security.JwtSecurity.Interfaces;

public interface ISecurity_JwtTokenHash
{
    Task<bool>
    ValidateToken
    (
       string tokenToValidate,
       string hashedTokenInDb
    );
}