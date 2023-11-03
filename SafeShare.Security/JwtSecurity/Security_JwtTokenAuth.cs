/*
 * Implementation of the ISecurity_JwtTokenAuth interface responsible for JWT token generation.
 * This service provides methods to create JWT tokens for user authentication, leveraging provided JWT settings.
*/

using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SafeShare.Utilities.ConfigurationSettings;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Security.JwtSecurity;

/// <summary>
/// Service for creating JWT tokens for authentication.
/// </summary>
public class Security_JwtTokenAuth : ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser>
{
    /// <summary>
    /// Represents the JWT authentication options.
    /// </summary>
    private readonly IOptions<Util_JwtSettings> _jwtOptions;
    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthJwtTokenService"/> class.
    /// </summary>
    /// <param name="jwtOptions">The JWT authentication options.</param>
    public Security_JwtTokenAuth
    (
        IOptions<Util_JwtSettings> jwtOptions
    )
    {
        _jwtOptions = jwtOptions;
    }
    /// <summary>
    /// Creates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user information. <see cref="UserDto"/></param>
    /// <returns>The generated JWT token.</returns>
    public string
    CreateToken
    (
        DTO_AuthUser user
    )
    {
        var singinCredentials = Security_JwtTokenGeneratorHelper.GetSinginCredentials(_jwtOptions.Value.Key);
        var claims = GetClaims(user);
        var token = Security_JwtTokenGeneratorHelper.GenerateToken(singinCredentials, claims, _jwtOptions.Value.Issuer, Convert.ToDouble(_jwtOptions.Value.LifeTime), true);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    /// <summary>
    /// Gets the claims for the specified user.
    /// </summary>
    /// <param name="user">The user information.</param>
    /// <returns>The list of claims.</returns>
    private List<Claim>
    GetClaims
    (
        DTO_AuthUser user
    )
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Value.Audience),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Value.Issuer),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.FullName),
            };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
}