using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SafeShare.DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;

namespace SafeShare.Security.JwtSecurity;

internal static class Security_JwtTokenGeneratorHelper
{
    /// <summary>
    /// Gets the signing credentials for JWT token generation.
    /// </summary>
    /// <returns>The signing credentials.</returns>
    internal static SigningCredentials
    GetSinginCredentials
    (
        string jwtKey
    )
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
    }
    /// <summary>
    /// Generates a JWT token with the specified signing credentials and claims.
    /// </summary>
    /// <param name="singinCredentials">The signing credentials.</param>
    /// <param name="claims">The list of claims.</param>
    /// <param name="duration">The time duration for token expiration</param>
    /// <param name="hours">An indicator if the token should expire by hours or not </param>
    /// <returns>The generated JWT token.</returns>
    internal static JwtSecurityToken
    GenerateToken
    (
        SigningCredentials singinCredentials,
        List<Claim> claims,
        string issuer,
        double duration,
        bool defaultToken
    )
    {
        var token = new JwtSecurityToken
        (
            issuer: issuer,
            claims: claims,
            expires: defaultToken ? DateTime.UtcNow.AddMinutes(duration) : DateTime.UtcNow.AddMinutes(duration),
            signingCredentials: singinCredentials
        );

        return token;
    }
}