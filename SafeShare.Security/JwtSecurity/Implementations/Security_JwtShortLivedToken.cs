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

public class Security_JwtShortLivedToken : ISecurity_JwtTokenAuth<Security_JwtShortLivedToken, string, string>
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<Util_JwtSettings> _jwtOptions;

    public Security_JwtShortLivedToken
    (
        IConfiguration configuration,
        IOptions<Util_JwtSettings> jwtOptions
    )
    {
        _configuration = configuration;
        _jwtOptions = jwtOptions;
    }

    public async Task<string>
    CreateToken
    (
        string userId
    )
    {
        var singinCredentials = Security_JwtTokenGeneratorHelper.GetSinginCredentials(_jwtOptions.Value.KeyConfrimLogin);
        var claims = GetClaims(userId);
        var token = Security_JwtTokenGeneratorHelper.GenerateToken(singinCredentials, claims, _jwtOptions.Value.Issuer, Convert.ToDouble(_configuration.GetSection("OTP_Duration").Value), false);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

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