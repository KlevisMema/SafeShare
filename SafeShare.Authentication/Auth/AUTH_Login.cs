/*
     * This class handles the user login functionality within 
     * the Authentication module. It takes care of authenticating 
     * the user based on the provided DTO and produces a JWT token upon successful authentication.
*/

using AutoMapper;
using SafeShare.Utilities.IP;
using System.Security.Claims;
using SafeShare.Utilities.Log;
using SafeShare.Utilities.User;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Email;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Services;
using Microsoft.AspNetCore.Identity;
using SafeShare.Utilities.Responses;
using Microsoft.EntityFrameworkCore;
using SafeShare.Utilities.Dependencies;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using Microsoft.Extensions.Configuration;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Security;
using SafeShare.Security.JwtSecurity.Interfaces;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.Security.JwtSecurity.Implementations;

namespace SafeShare.Authentication.Auth;

/// <summary>
///     Provides functionality to authenticate and log in users within the Authentication module.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="AUTH_Login"/> class.
/// </remarks>
/// <param name="mapper">The mapper.</param>
/// <param name="logger">The logger.</param>
/// <param name="userManager">The user manager.</param>
/// <param name="configuration">The configurations </param>
/// <param name="signInManager">The sign-in manager.</param>
/// <param name="jwtTokenService">The JWT token service.</param>
/// <param name="jwtShortTokenService">The JWT token service.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
/// <param name="db">The application db context for db operations</param>
public class AUTH_Login
(
    IMapper mapper,
    ApplicationDbContext db,
    ILogger<AUTH_Login> logger,
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser, DTO_Token> jwtTokenService,
    ISecurity_JwtTokenAuth<Security_JwtShortLivedToken, string, string> jwtShortTokenService
) : Util_BaseAuthDependencies<AUTH_Login, ApplicationUser, ApplicationDbContext>(
    mapper,
    logger,
    httpContextAccessor,
    userManager,
    configuration,
    db
), IAUTH_Login
{
    /// <summary>
    ///     Authenticates and logs in a user based on the provided login data transfer object.
    /// </summary>
    /// <param name="loginDto">The data transfer object containing user login details.</param>
    /// <returns>A generic response with a JWT token (if require otp during login is false) or an error message.</returns>
    public async Task<Util_GenericResponse<DTO_LoginResult>>
    LoginUser
    (
        DTO_Login loginDto
    )
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            user ??= await _userManager.FindByNameAsync(loginDto.Email);

            if (user is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [Authentication Module]-[AUTH_Login Class]-[LoginUser Method] =>
                         [RESULT] : [IP] {IP} user doesnt exists. DTO {@DTO}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    loginDto
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "Invalid credentials.",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            if (!emailConfirmed)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                          [Authentication Module]-[AUTH_Login Class]-[LoginUser Method] =>  
                          user {Email} was not logged in =>  [RESULT] :  
                          [IP] {IP} users email is not verified.
                     """,
                    loginDto.Email,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor)
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "Your email is not verified",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var signInUser = await signInManager.PasswordSignInAsync(user, loginDto.Password, true, lockoutOnFailure: true);

            if (signInUser.IsLockedOut)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                          [Authentication Module]-[AUTH_Login Class]-[LoginUser Method] =>  
                          user {Email} was not logged in =>  [RESULT] :  
                          [IP] {IP}, is user locked out : {signInUser.IsLockedOut} |
                          User {Email} is locked.
                     """,
                    loginDto.Email,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    signInUser.IsLockedOut,
                    loginDto.Email
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    $"You are locked out, please wait for {await GetLockoutTimeRemaining(user)} and try again!",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            if (!signInUser.Succeeded)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [Authentication Module]-[AUTH_Login Class]-[LoginUser Method] =>  
                         user {loginDto.Email} was not logged in =>  [RESULT] :  
                         [IP] {IP} | Invalid Credentials.
                         {@signInUser}
                     """,
                    loginDto.Email,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    signInUser
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "Invalid credentials!",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            if (user.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                          [Authentication Module]-[AUTH_Login Class]-[LoginUser Method] =>  
                          [IP] {IP}, user was not logged in => account {Id} is deactivated.
                          User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    user.Id,
                    user
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    """
                         Something went wrong when loggining in the account,
                         your account exists and is deactivated.  
                         Please reactivate it.
                     """,
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var loginResult = new DTO_LoginResult();

            if (user.RequireOTPDuringLogin)
            {
                user.OTP_Duration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration.GetSection("OTP_Duration").Value!));
                user.OTP = Guid.NewGuid().ToString()[..8];
                await _userManager.UpdateAsync(user);
                await Util_Email.SendOTP_Email(user.Email!, user.FullName, user.OTP!);
            }
            else
            {
                user.LastLogIn = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            if (user.RequireOTPDuringLogin)
            {
                loginResult.Token.RefreshToken = null;
                loginResult.Token.Token = await GetShortToken(user.Id);
            }
            else
                loginResult.Token = await GetToken(user);

            _logger.LogInformation
            (
                """
                    [Authentication Module]-[AUTH_Login Class]-[LoginUser Method] => 
                    [IP] {IP} | user {loginDto.Email} credentials valiadted successfully."
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                loginDto.Email
            );

            loginResult.RequireOtpDuringLogin = user.RequireOTPDuringLogin;
            loginResult.UserId = user.Id;

            return Util_GenericResponse<DTO_LoginResult>.Response
            (
                loginResult,
                true,
                "Credentials succsessfully validated!",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_LoginResult, AUTH_Login>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [Authentication Module]-[AUTH_Login Class]-[LoginUser Method], 
                    user with [EMAIL] {loginDto.Email}.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    ///     Confirm the login of the user
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="otp">The one time password </param>
    /// <returns>A generic respone with jwt token or a error message</returns>
    public async Task<Util_GenericResponse<DTO_LoginResult>>
    ConfirmLogin
    (
        Guid userId,
        string otp
    )
    {
        try
        {
            if (String.IsNullOrEmpty(otp))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[ConfirmLogin Method] => 
                        [RESULT] : [IP] {IP}    
                        user with [ID] {userId} put an empty ot 
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "OTP empty, please provide the otp",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[ConfirmLogin Method] => 
                        [RESULT] : [IP] {IP}  
                        user with [ID] {userId} doesnt exists   
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "User doesn't exists",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            if (!user.RequireOTPDuringLogin)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[ConfirmLogin Method] => 
                        [RESULT] : [IP] {IP}  
                        user with [ID] {userId} tried to confrim his login but has  
                        not the flag true to require otp during login.  
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "You don't require otp during login.",
                    null, System.Net.HttpStatusCode.BadRequest
                );
            }

            if (user.OTP_Duration is not null && DateTime.UtcNow > user.OTP_Duration)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        $"[Authentication Module]-[AUTH_Login Class]-[ConfirmLogin Method] => "
                        $"[Result] : [IP] {IP} "
                        $"User with [ID] {userId} otp expired"
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "Your one time password has expired",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            if (user.OTP != otp)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[ConfirmLogin Method] => 
                        [Result] : [IP] {IP}  
                        User with [ID] {userId} put wrong opt, opt was not verified 
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    "Wrong otp!",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            if (user.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[ConfirmLogin Method] => 
                        [IP] {IP} user was not logged in => account {ID} is deactivated.  
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    user.Id
                );

                return Util_GenericResponse<DTO_LoginResult>.Response
                (
                    null,
                    false,
                    """
                        Something went wrong when loggining in the account,
                        your account exists and is deactivated. Please reactivate it.
                     """,
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var confirmLoginResult = new DTO_LoginResult
            {
                RequireOtpDuringLogin = true,
                Token = await GetToken(user),
                UserId = user.Id
            };

            user.OTP = null;
            user.OTP_Duration = null;
            user.LastLogIn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.Log
            (
                LogLevel.Information,
                """
                    [Authentication Module]-[AUTH_Login Class]-[ConfirmLogin Method] => 
                    [IP] {IP} user [ID] {ID} was succsessfully logged.   
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                user.Id
            );

            return Util_GenericResponse<DTO_LoginResult>.Response
            (
                confirmLoginResult,
                true,
                "OTP validated succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_LoginResult, AUTH_Login>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [Authentication Module]-
                    [AUTH_Login Class]-[ConfirmLogin Method], user with [ID] {userId}.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    ///     Log out a user
    /// </summary>
    /// <returns> Asyncronous Task</returns>
    public async Task
    LogOut
    (
        string userId
    )
    {
        var userIdFromToken = Util_FindUserIdFromToken.UserId(_httpContextAccessor);

        try
        {
            if (userId != userIdFromToken)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[LogOut Method] =>       
                        [RESULT] : [IP] {IP} user with [ID] {userId} is not equal with 
                        [Id in token] {userIdFromToken}. 
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId,
                    userIdFromToken
                );

                return;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[LogOut Method] =>   
                        [RESULT] : [IP] {IP}  
                        user with [ID] {userId} doesn't exists. 
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );

                return;
            }

            user.LastLogOut = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[LogOut Method] =>   
                        [RESULT] : [IP] {IP}  
                        user with [ID] {userId} update failed during logout.    
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );
            }

            var userTokens = await _db.RefreshTokens.Include(x => x.User)
                                                    .Where(x => x.UserId == user.Id)
                                                    .ToArrayAsync();

            if (userIdFromToken is not null)
            {
                _db.RefreshTokens.RemoveRange(userTokens);
                await _db.SaveChangesAsync();

                _logger.Log
                (
                   LogLevel.Information,
                   """
                        [Authentication Module]-[AUTH_Login Class]-[LogOut Method] =>   
                        [RESULT] : [IP] {IP}    
                        user with [ID] {userId} succsessfully logged out.   
                    """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );
            }
            else
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [Authentication Module]-[AUTH_Login Class]-[LogOut Method] =>   
                        [RESULT] : [IP] {IP}  
                        user with [ID] {userId} doesn't have any refresh tokens, the delete 
                        process failed. 
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    userId
                );
            }

        }
        catch (Exception ex)
        {
            _logger.LogCritical
            (
                ex,
                """
                    Something went wrong in [Authentication Module]-
                    [AUTH_Login Class]-[LogOut Method], [IP] {IP}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor)
            );
        }
    }
    /// <summary>
    ///     Get the time remaining a user is locked out.
    /// </summary>
    /// <param name="user"> The application user</param>
    /// <returns> The time remaining the user has its account locked</returns>
    private async Task<TimeSpan?>
    GetLockoutTimeRemaining
    (
        ApplicationUser user
    )
    {
        var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
        var lockoutTimeRemaining = lockoutEnd?.UtcDateTime.Subtract(DateTime.UtcNow);

        return lockoutTimeRemaining;
    }
    /// <summary>
    ///     Get the jwt token.
    /// </summary>
    /// <param name="user">The <see cref="ApplicationUser"/> object </param>
    /// <returns> The Jwt token </returns>
    private async Task<DTO_Token>
    GetToken
    (
        ApplicationUser user
    )
    {
        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<DTO_AuthUser>(user);
        userDto.Roles = roles.ToList();
        var token = await jwtTokenService.CreateToken(userDto);

        return token;
    }
    /// <summary>
    ///     Get the short jwt token.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns> The Jwt token </returns>
    private async Task<string>
    GetShortToken
    (
        string userId
    )
    {
        return await jwtShortTokenService.CreateToken(userId);
    }
}