using AutoMapper;
using System.Security.Claims;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Security;
using SafeShare.Security.JwtSecurity.Interfaces;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.Security.JwtSecurity.Implementations;

namespace SafeShare.Authentication.Auth;

public class AUTH_RefreshToken : IAUTH_RefreshToken
{
    /// <summary>
    /// 
    /// </summary>
    private readonly TokenValidationParameters _tokenValidationParameters;
    /// <summary>
    /// 
    /// </summary>
    private readonly ApplicationDbContext _db;
    /// <summary>
    /// 
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;
    /// <summary>
    /// 
    /// </summary>
    private readonly ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser, DTO_Token> _jwtTokenService;
    /// <summary>
    /// 
    /// </summary>
    private readonly IMapper _mapper;
    /// <summary>
    /// 
    /// </summary>
    private readonly ISecurity_JwtTokenHash _securityJwtTokenHash;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenValidationParameters"></param>
    /// <param name="db"></param>
    /// <param name="userManager"></param>
    public AUTH_RefreshToken
    (
        IMapper mapper,
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        ISecurity_JwtTokenHash securityJwtTokenHash,
        TokenValidationParameters tokenValidationParameters,
        ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser, DTO_Token> jwtTokenService
    )
    {
        _db = db;
        _mapper = mapper;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _securityJwtTokenHash = securityJwtTokenHash;
        _tokenValidationParameters = tokenValidationParameters;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="validateTokenDto"></param>
    /// <returns></returns>
    public async Task<Util_GenericResponse<DTO_Token>>
    RefreshToken
    (
        DTO_ValidateToken validateTokenDto
    )
    {
        try
        {
            var claimsPrincipal = GetClaimsPrincipal(validateTokenDto.Token);

            if (claimsPrincipal is null)
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Invalid token", null, System.Net.HttpStatusCode.Unauthorized);
            }

            var expiryDateUnix = long.Parse(claimsPrincipal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Token has not expired yet", null, System.Net.HttpStatusCode.BadRequest);
            }

            var jti = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value; // id of the token

            var storedRefreshToken = await _db.RefreshTokens.SingleOrDefaultAsync(x => x.Id == validateTokenDto.RefreshTokenId);

            if (storedRefreshToken is null)
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Refresh token does not exists", null, System.Net.HttpStatusCode.NotFound);
            }

            if (!await _securityJwtTokenHash.ValidateToken(validateTokenDto.RefreshToken, storedRefreshToken.HashedToken))
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Invalid Refresh token", null, System.Net.HttpStatusCode.NotFound);
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Refresh token has expired", null, System.Net.HttpStatusCode.NotFound);
            }

            if (storedRefreshToken.Invaidated)
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Refresh token has been invalidated", null, System.Net.HttpStatusCode.BadRequest);
            }

            if (storedRefreshToken.Used)
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Refresh token has been used", null, System.Net.HttpStatusCode.BadRequest);
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return Util_GenericResponse<DTO_Token>.Response(null, false, "Refresh token does not match the jti", null, System.Net.HttpStatusCode.BadRequest);
            }

            storedRefreshToken.Used = true;

            _db.RefreshTokens.Update(storedRefreshToken);
            await _db.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(claimsPrincipal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<DTO_AuthUser>(user);
            userDto.Roles = roles.ToList();
            var token = await _jwtTokenService.CreateToken(userDto);

            return Util_GenericResponse<DTO_Token>.Response(token, true, "Refresh token issued succsessfully", null, System.Net.HttpStatusCode.OK);


        }
        catch (Exception ex)
        {

            throw;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private ClaimsPrincipal?
    GetClaimsPrincipal
    (
        string token
    )
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                return null;

            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            // Handle the expired token. If you need to extract claims from an expired token you can do so here.
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null) return null;

            var claimsIdentity = new ClaimsIdentity(jwtToken.Claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return claimsPrincipal;
        }
        catch
        {
            // Handle other types of exceptions or re-throw.
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="validatedToken"></param>
    /// <returns></returns>
    private bool
    IsJwtWithValidSecurityAlgorithm
    (
        SecurityToken validatedToken
    )
    {
        return (validatedToken is JwtSecurityToken jwtSecurity) &&
                jwtSecurity.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature,
                StringComparison.InvariantCultureIgnoreCase);
    }
}