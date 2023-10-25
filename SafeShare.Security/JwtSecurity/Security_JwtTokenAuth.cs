/*
 * Implementation of the ISecurity_JwtTokenAuth interface responsible for JWT token generation.
 * This service provides methods to create JWT tokens for user authentication, leveraging provided JWT settings.
*/

using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Security.JwtSecurity;

/// <summary>
/// Service for creating JWT tokens for authentication.
/// </summary>
public class Security_JwtTokenAuth : ISecurity_JwtTokenAuth
{
    /// <summary>
    /// Represents the JWT authentication options.
    /// </summary>
    private readonly IOptions<Security_JwtSettings> _jwtOptions;
    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthJwtTokenService"/> class.
    /// </summary>
    /// <param name="jwtOptions">The JWT authentication options.</param>
    public Security_JwtTokenAuth
    (
        IOptions<Security_JwtSettings> jwtOptions
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
        var singinCredentials = GetSinginCredentials();
        var claims = GetClaims(user);
        var token = GenerateToken(singinCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    /// <summary>
    /// Gets the signing credentials for JWT token generation.
    /// </summary>
    /// <returns>The signing credentials.</returns>
    private SigningCredentials
    GetSinginCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));

        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
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
                new Claim(JwtRegisteredClaimNames.NameId, user.Id!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Value.Audience),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Value.Issuer),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.FullName),
            };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
    /// <summary>
    /// Generates a JWT token with the specified signing credentials and claims.
    /// </summary>
    /// <param name="singinCredentials">The signing credentials.</param>
    /// <param name="claims">The list of claims.</param>
    /// <returns>The generated JWT token.</returns>
    private JwtSecurityToken
    GenerateToken
    (
        SigningCredentials singinCredentials,
        List<Claim> claims
    )
    {
        var token = new JwtSecurityToken
        (
            issuer: _jwtOptions.Value.Issuer,
            claims: claims,
            expires: DateTime.Now.AddHours(Convert.ToDouble(_jwtOptions.Value.LifeTime)),
            signingCredentials: singinCredentials
        );

        return token;
    }
}