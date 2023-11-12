﻿/* 
 * Provides implementation for the ISecurity_JwtTokenAuth interface, responsible for JWT token generation.
 * This class offers methods to create JWT tokens for user authentication, utilizing JWT settings provided through configuration.
 * It also includes functionality for validating JWT tokens and adding them to the database.
 */

using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.DataTransormObject.Security;
using SafeShare.Utilities.ConfigurationSettings;
using SafeShare.Security.JwtSecurity.Interfaces;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Security.JwtSecurity.Implementations;

/// <summary>
/// Service responsible for creating and validating JWT tokens for authentication.
/// </summary>
public class Security_JwtTokenAuth : ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser, DTO_Token>, ISecurity_JwtTokenHash
{
    /// <summary>
    /// The database context
    /// </summary>
    private readonly ApplicationDbContext _db;
    /// <summary>
    /// Represents the JWT authentication options.
    /// </summary>
    private readonly IOptions<Util_JwtSettings> _jwtOptions;
    /// <summary>
    /// A Instance of <see cref="HashAlgorithm"/>
    /// </summary>
    private readonly HashAlgorithm _hashAlgorithm = SHA256.Create();
    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthJwtTokenService"/> class.
    /// </summary>
    /// <param name="db">The database context</param>
    /// <param name="jwtOptions">The JWT authentication options.</param>
    public Security_JwtTokenAuth
    (
        ApplicationDbContext db,
        IOptions<Util_JwtSettings> jwtOptions
    )
    {
        _db = db;
        _jwtOptions = jwtOptions;
    }
    /// <summary>
    /// Creates a JWT token based on the specified input parameter.
    /// </summary>
    /// <param name="user">The input parameter used for token generation. This could be user information or any other required data.</param>
    /// <returns>A task representing the asynchronous operation. The task result is of type <see cref="DTO_Token"/>, representing the generated JWT token.</returns>
    public async Task<DTO_Token>
    CreateToken
    (
        DTO_AuthUser user
    )
    {
        var singinCredentials = Security_JwtTokenGeneratorHelper.GetSinginCredentials(_jwtOptions.Value.Key);
        var claims = GetClaims(user);
        var token = Security_JwtTokenGeneratorHelper.GenerateToken(singinCredentials, claims, _jwtOptions.Value.Issuer, Convert.ToDouble(_jwtOptions.Value.LifeTime), true);

        var tokenDto = await AddToDb(token.Id, claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
        tokenDto.Token = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenDto;
    }
    /// <summary>
    /// Validates a JWT token against a hashed version stored in the database.
    /// </summary>
    /// <param name="tokenToValidate">The JWT token to validate.</param>
    /// <param name="hashedTokenInDb">The hashed version of the token stored in the database.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a boolean indicating the validity of the token.</returns>
    public Task<bool>
    ValidateToken
    (
        string tokenToValidate,
        string hashedTokenInDb
    )
    {
        string hashedTokenToValidate = Convert.ToBase64String(_hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(tokenToValidate)));

        if (hashedTokenInDb == hashedTokenToValidate)
            return Task.FromResult(true);

        return Task.FromResult(false);
    }
    /// <summary>
    /// Add the refresh token in the database
    /// </summary>
    /// <param name="tokenId">The id of the token</param>
    /// <param name="userId">The id of the user</param>
    /// <returns>A <see cref="DTO_Token"/> object </returns>
    private async Task<DTO_Token>
    AddToDb
    (
        string tokenId,
        string userId
    )
    {
        try
        {
            var randomId = Guid.NewGuid().ToString();
            var hashedRefreshToken = Convert.ToBase64String(_hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(randomId)));

            var refreshToken = new RefreshToken
            {
                HashedToken = hashedRefreshToken,
                JwtId = tokenId,
                CreationDate = DateTime.UtcNow,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.Value.LifeTime),
            };

            await _db.RefreshTokens.AddAsync(refreshToken);

            await _db.SaveChangesAsync();

            return new DTO_Token { RefreshToken = randomId, RefreshTokenId = refreshToken.Id, CreatedAt = refreshToken.CreationDate };
        }
        catch (Exception ex)
        {

            throw;
        }
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