/*
     * This class handles the user login functionality within 
     * the Authentication module. It takes care of authenticating 
     * the user based on the provided DTO and produces a JWT token upon successful authentication.
*/

using AutoMapper;
using SafeShare.Utilities.IP;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using SafeShare.Utilities.Responses;
using SafeShare.Security.JwtSecurity;
using SafeShare.DataAccessLayer.Models;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Authentication.Auth;

/// <summary>
/// Provides functionality to authenticate and log in users within the Authentication module.
/// </summary>
public class AUTH_Login : IAUTH_Login
{
    /// <summary>
    /// Mapper instance to map between different object types.
    /// </summary>
    private readonly IMapper _mapper;
    /// <summary>
    /// Logger instance to log information and errors.
    /// </summary>
    private readonly ILogger<AUTH_Login> _logger;
    /// <summary>
    /// Manager to handle user-related operations.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;
    /// <summary>
    /// Service to handle JWT token operations.
    /// </summary>
    private readonly ISecurity_JwtTokenAuth _jwtTokenService;
    /// <summary>
    /// Manager to handle user sign-in operations.
    /// </summary>
    private readonly SignInManager<ApplicationUser> _signInManager;
    /// <summary>
    /// Accessor to get information about the current HTTP context.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;
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
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
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

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            if (!emailConfirmed)
                return Util_GenericResponse<string>.Response(null, false, "Your email is not verified", null, System.Net.HttpStatusCode.BadRequest);

            var signInUser = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if (!signInUser.Succeeded)
                return Util_GenericResponse<string>.Response(string.Empty, false, "Invalid credentials!", null, System.Net.HttpStatusCode.NotFound);

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<DTO_AuthUser>(user);
            userDto.Roles = roles.ToList();
            var token = _jwtTokenService.CreateToken(userDto);

            _logger.LogInformation($"[Authentication Module] - [LoginUser Method], {Util_GetIpAddres.GetLocation(_httpContextAccessor)} user {loginDto.Email} credentials valiadted successfully.");

            return Util_GenericResponse<string>.Response(token, true, "User data succsessfully validated!", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            var errorResponse = Util_GenericResponse<string>.Response(string.Empty, false, ex.ToString(), null, System.Net.HttpStatusCode.InternalServerError);

            _logger.LogError(ex, $"Somewthing went wrong in [Authentication Module] - [LoginUser Method], user with Ip {Util_GetIpAddres.GetLocation(_httpContextAccessor)}", errorResponse);

            errorResponse.Message = "Internal server error";

            return errorResponse;
        }
    }
}