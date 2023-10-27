/*
     * This class handles the user login functionality within 
     * the Authentication module. It takes care of authenticating 
     * the user based on the provided DTO and produces a JWT token upon successful authentication.
*/

using AutoMapper;
using SafeShare.Utilities.IP;
using SafeShare.Utilities.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Services;
using Microsoft.AspNetCore.Identity;
using SafeShare.Utilities.Responses;
using SafeShare.Security.JwtSecurity;
using SafeShare.Utilities.Dependencies;
using SafeShare.DataAccessLayer.Models;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.Authentication.Auth;

/// <summary>
/// Provides functionality to authenticate and log in users within the Authentication module.
/// </summary>
public class AUTH_Login : Util_BaseAuthDependencies<AUTH_Login, ApplicationUser>, IAUTH_Login
{
    /// <summary>
    /// Service to handle JWT token operations.
    /// </summary>
    private readonly ISecurity_JwtTokenAuth _jwtTokenService;
    /// <summary>
    /// Manager to handle user sign-in operations.
    /// </summary>
    private readonly SignInManager<ApplicationUser> _signInManager;
    /// <summary>
    /// Initializes a new instance of the <see cref="AUTH_Login"/> class.
    /// </summary>
    /// <param name="mapper">The mapper.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="jwtTokenService">The JWT token service.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public AUTH_Login
    (
        IMapper mapper,
        ILogger<AUTH_Login> logger,
        UserManager<ApplicationUser> userManager,
        ISecurity_JwtTokenAuth jwtTokenService,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor
    ) 
    : base
    (
        mapper, 
        logger, 
        httpContextAccessor, 
        userManager
    )
    {
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }
    /// <summary>
    /// Authenticates and logs in a user based on the provided login data transfer object.
    /// </summary>
    /// <param name="loginDto">The data transfer object containing user login details.</param>
    /// <returns>A generic response with a JWT token (if successful) or an error message.</returns>
    public async Task<Util_GenericResponse<string>>
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
                return Util_GenericResponse<string>.Response(string.Empty, false, "User doesn't exists!", null, System.Net.HttpStatusCode.NotFound);

            //var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            //if (!emailConfirmed)
            //    return Util_GenericResponse<string>.Response(null, false, "Your email is not verified", null, System.Net.HttpStatusCode.BadRequest);

            var signInUser = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if (!signInUser.Succeeded)
                return Util_GenericResponse<string>.Response(string.Empty, false, "Invalid credentials!", null, System.Net.HttpStatusCode.NotFound);

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<DTO_AuthUser>(user);
            userDto.Roles = roles.ToList();
            var token = _jwtTokenService.CreateToken(userDto);

            _logger.LogInformation($"[Authentication Module] - [LoginUser Method] => , [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} | user {loginDto.Email} credentials valiadted successfully.");

            return Util_GenericResponse<string>.Response(token, true, "User data succsessfully validated!", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<string, AUTH_Login>.ReturnInternalServerError(ex, _logger, $"Somewthing went wrong in [Authentication Module] - [LoginUser Method], user with [EMAIL] {loginDto.Email}", string.Empty, _httpContextAccessor);
        }
    }
}