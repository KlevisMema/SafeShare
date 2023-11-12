/* 
 * Implements the functionality to create short-lived JWT (JSON Web Tokens) for authentication purposes.
 * This class provides methods to generate tokens with a limited lifespan, typically used for operations like one-time password (OTP) authentication.
 */

using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using SafeShare.Utilities.ConfigurationSettings;
using SafeShare.Security.JwtSecurity.Interfaces;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Security.JwtSecurity.Implementations;

/// <summary>
/// Implements the generation of short-lived JWT tokens for specific authentication scenarios.
/// </summary>
public class Security_JwtShortLivedToken : ISecurity_JwtTokenAuth<Security_JwtShortLivedToken, string, string>
{
    /// <summary>
    /// The application configuration to access settings like OTP duration.
    /// </summary>
    private readonly IConfiguration _configuration;
    /// <summary>
    /// The JWT settings to be used for token generation.
    /// </summary>
    private readonly IOptions<Util_JwtSettings> _jwtOptions;
    /// <summary>
    /// Initializes a new instance of the <see cref="Security_JwtShortLivedToken"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration to access settings like OTP duration.</param>
    /// <param name="jwtOptions">The JWT settings to be used for token generation.</param>
    public Security_JwtShortLivedToken
    (
        IConfiguration configuration,
        IOptions<Util_JwtSettings> jwtOptions
    )
    {
        _configuration = configuration;
        _jwtOptions = jwtOptions;
    }
    /// <summary>
    /// Creates a short-lived JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier for whom the token is being created.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the generated JWT token string.</returns>
    public Task<string>
    CreateToken
    (
        string userId
    )
    {
        var singinCredentials = Security_JwtTokenGeneratorHelper.GetSinginCredentials(_jwtOptions.Value.KeyConfrimLogin);
        var claims = GetClaims(userId);
        var token = Security_JwtTokenGeneratorHelper.GenerateToken(singinCredentials, claims, _jwtOptions.Value.Issuer, Convert.ToDouble(_configuration.GetSection("OTP_Duration").Value), false);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
    /// <summary>
    /// Gets the list of claims to be included in the JWT token.
    /// </summary>
    /// <param name="userId">The user identifier whose claims are being prepared.</param>
    /// <returns>A list of claims for the specified user.</returns>
    private List<Claim>
    GetClaims
    (
        string userId
    )
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Value.Audience),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Value.Issuer)
        };

        return claims;
    }
}