using System.Text;
using System.Security.Claims;
using API_Client.BLL.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SafeShare.Mappings.SafeShareApiKey;
using SafeShare.Utilities.SafeShareApiKey.Helpers;
using SafeShare.DataAccessLayer.Models.SafeShareApiKey;

namespace API_Client.BLL.Implementation;

public class AuthenticationService
(
    SignInManager<ApiClient> signInManager,
    UserManager<ApiClient> userManager,
    ILogger<AuthenticationService> logger,
    IOptions<JwtSettings> _jwtSettings
) : IAuthenticationService
{
    public async Task<ServiceResponse<string>> 
    Login
    (
        string userName, 
        string password
    )
    {
        try
        {
            logger.LogInformation("Attempting to log in user {UserName}", userName);
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                logger.LogWarning("Login failed for user {UserName}: User not found", userName);
                return ServiceResponse<string>.Fail("User not found");
            }

            if (!user.IsActive)
            {
                logger.LogWarning("Login failed for user {UserName}: User not active", userName);
                return ServiceResponse<string>.Fail("User not active");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);

            if (result.Succeeded)
            {
                logger.LogInformation("User {UserName} logged in successfully", userName);
                return new ServiceResponse<string>(GenerateJwtToken(user), "Login Succsessfull");
            }

            logger.LogWarning("Login attempt failed for user {UserName}", userName);
            return ServiceResponse<string>.Fail("Invalid credentials");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while logging in user {UserName}", userName);
            return ServiceResponse<string>.Fail("An unexpected error occurred");
        }
    }

    private string 
    GenerateJwtToken
    (
        ApiClient user
    )
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id)
            }),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.Value.LifeTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Value.Issuer,
            Audience = _jwtSettings.Value.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}