/*
 * The AUTH_Register class is part of the Authentication module and provides functionalities
 * for user registration. This includes the main process of registering a user and
 * assigning roles to the newly registered user. Error handling and logging are integral
 * to these operations to ensure smooth user registration flow.
 */

using AutoMapper;
using SafeShare.Utilities.IP;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Authentication.Auth;

/// <summary>
/// Handles user registration within the authentication module.
/// </summary>
public class AUTH_Register : IAUTH_Register
{
    /// <summary>
    /// Provides mapping capabilities between different object types.
    /// </summary>
    private readonly IMapper _mapper;
    /// <summary>
    /// The primary database context for the application.
    /// </summary>
    private readonly ApplicationDbContext _db;
    /// <summary>
    /// Logger instance for logging events and exceptions in the AUTH_Register class.
    /// </summary>
    private readonly ILogger<AUTH_Register> _logger;
    /// <summary>
    /// Provides functionalities related to user management such as creating, updating, and deleting users.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;
    /// <summary>
    /// Allows access to the HTTP context of the current request.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;
    /// <summary>
    /// Initializes a new instance of the <see cref="AUTH_Register"/> class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="db">The application's database context.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="httpContextAccessor">The HttpContext accessor instance.</param>
    /// <param name="userManager">The user manager instance.</param>
    public AUTH_Register
    (
        IMapper mapper,
        ApplicationDbContext db,
        ILogger<AUTH_Register> logger,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager
    )
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// Registers a new user to the application.
    /// </summary>
    /// <param name="registerDto">The data transfer object containing user registration details.</param>
    /// <returns>A generic response indicating the success or failure of the registration.</returns>
    public async Task<Util_GenericResponse<bool>>
    RegisterUser
    (
        DTO_Register registerDto
    )
    {
        try
        {
            var test = _mapper.Map<ApplicationUser>(registerDto);

            var createUserResult = await _userManager.CreateAsync(test, registerDto.Password);

            if (!createUserResult.Succeeded)
                return Util_GenericResponse<bool>.Response(false, false, "Something went wrong when creating the user!", createUserResult.Errors.Select(x => x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);

            var assignRole = await AssignUserToUserRole(registerDto.UserName);

            if (!assignRole.Succsess)
                return assignRole;

            _logger.LogInformation($"[Authentication Module] - [RegisterUser Method], {Util_GetIpAddres.GetLocation(_httpContextAccessor)} user {@createUserResult} was succsessfully created created.");

            return Util_GenericResponse<bool>.Response(true, true, "Your account was successfully created", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            var errorResponse = Util_GenericResponse<bool>.Response(false, false, ex.ToString(), null, System.Net.HttpStatusCode.InternalServerError);

            _logger.LogError(ex, $"Somewthing went wrong in [Authentication Module] - [RegisterUser Method], user with Ip {Util_GetIpAddres.GetLocation(_httpContextAccessor)}", errorResponse);

            errorResponse.Message = "Internal server error.";

            return errorResponse;
        }
    }
    /// <summary>
    /// Assigns a registered user to a predefined 'User' role.
    /// </summary>
    /// <param name="userName">The username of the registered user.</param>
    /// <returns>A generic response indicating the success or failure of the role assignment.</returns>
    private async Task<Util_GenericResponse<bool>>
    AssignUserToUserRole
    (
        string userName
    )
    {
        try
        {
            var user = await _userManager.FindByNameAsync(userName);

            var roles = await _db.Roles.ToListAsync();

            if (roles.Count > 0 && user is not null)
            {
                string? userRole = roles?.FirstOrDefault(role => role.Name == "User")?.Name;

                if (String.IsNullOrEmpty(userRole))
                {
                    await _userManager.DeleteAsync(user);
                    return Util_GenericResponse<bool>.Response(false, false, "Something went wrong in assigning the role to the user, user was not created!", null, System.Net.HttpStatusCode.BadRequest);
                }

                var addToRole = await _userManager.AddToRoleAsync(user!, userRole);

                if (addToRole is null)
                {
                    await _userManager.DeleteAsync(user);
                    return Util_GenericResponse<bool>.Response(false, false, "Something went wrong in assigning the role to the user, user was not created!", null, System.Net.HttpStatusCode.BadRequest);
                }
            }

            return Util_GenericResponse<bool>.Response(true, true, "User succsessfully assigned to user role", null, System.Net.HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            var errorResponse = Util_GenericResponse<bool>.Response(false, false, ex.ToString(), null, System.Net.HttpStatusCode.InternalServerError);

            _logger.LogError(ex, $"Somewthing went wrong in [Authentication Module] - [AssignUserToUserRole Method], user with Ip {Util_GetIpAddres.GetLocation(_httpContextAccessor)}", errorResponse);

            errorResponse.Message = "Internal server error.";

            return errorResponse;
        }
    }
}