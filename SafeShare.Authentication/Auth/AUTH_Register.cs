/*
 * The AUTH_Register class is part of the Authentication module and provides functionalities
 * for user registration. This includes the main process of registering a user and
 * assigning roles to the newly registered user. Error handling and logging are integral
 * to these operations to ensure smooth user registration flow.
 */

using AutoMapper;
using SafeShare.Utilities.IP;
using SafeShare.Utilities.Log;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Email;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Services;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using SafeShare.Utilities.Dependencies;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using Microsoft.Extensions.Configuration;
using SafeShare.Authentication.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SafeShare.Utilities.ConfigurationSettings;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Authentication.Auth;

/// <summary>
/// Handles user registration within the authentication module.
/// </summary>
public class AUTH_Register : Util_BaseAuthDependencies<AUTH_Register, ApplicationUser>, IAUTH_Register
{
    /// <summary>
    /// The primary database context for the application.
    /// </summary>
    private readonly ApplicationDbContext _db;
    /// <summary>
    /// The confirm registration settings
    /// </summary>
    private readonly IOptions<Util_ConfirmRegistrationSettings> _confirmRegistrationSettings;
    /// <summary>
    /// Initializes a new instance of the <see cref="AUTH_Register"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="configuration"> The configurations </param>
    /// <param name="db">The application's database context.</param>
    /// <param name="userManager">The user manager instance.</param>
    /// <param name="httpContextAccessor">The HttpContext accessor instance.</param>
    /// <param name="confirmRegistrationSettings">The confirm registration settings</param>
    public AUTH_Register
    (
        IMapper mapper,
        ApplicationDbContext db,
        ILogger<AUTH_Register> logger,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        IOptions<Util_ConfirmRegistrationSettings> confirmRegistrationSettings
    )
    : base
    (
        mapper,
        logger,
        httpContextAccessor,
        userManager,
        configuration
    )
    {
        _db = db;
        _confirmRegistrationSettings = confirmRegistrationSettings;
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
            var mappedUser = _mapper.Map<ApplicationUser>(registerDto);

            var createUserResult = await _userManager.CreateAsync(mappedUser, registerDto.Password);

            if (!createUserResult.Succeeded)
            {
                _logger.Log(LogLevel.Information, $"[RegisterUser Method] => [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user was not created Created =>  [RESULT] : {createUserResult.Succeeded} and {@createUserResult.Errors.Select(x => x.Description)}");
                return Util_GenericResponse<bool>.Response(false, false, "Something went wrong when creating the account!", createUserResult.Errors.Select(x => x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
            }

            var assignRole = await AssignUserToUserRole(registerDto.UserName);

            if (!assignRole.Succsess)
            {
                _logger.Log(LogLevel.Information, $"[RegisterUser Method] => [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user was not assigend to a role =>  [RESULT] : {assignRole.Succsess} and {assignRole.Message}");
                return assignRole;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(mappedUser);

            var route = _confirmRegistrationSettings.Value.Route.Replace("{token}", token).Replace("{email}", mappedUser.Email);

            var emailResult = await Util_Email.SendEmailForRegistrationConfirmation(mappedUser.Email!, route, mappedUser.FullName);

            if (!emailResult.IsSuccessStatusCode)
            {
                // do smth
            }

            _logger.LogInformation($"[Authentication Module] - [RegisterUser Method] =>, [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} | user {registerDto.Email} was succsessfully created created.");

            return Util_GenericResponse<bool>.Response(true, true, "Your account was successfully created, we have send you an email to confirm your account.", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AUTH_Register>.ReturnInternalServerError(ex, _logger, $"Somewthing went wrong in [Authentication Module] - [RegisterUser Method], user with [EMAIL] {registerDto.Email}", false, _httpContextAccessor);
        }
    }
    /// <summary>
    /// Confirms user registration
    /// </summary>
    /// <param name="confirmRegistrationDto"> The <see cref="DTO_ConfirmRegistration"/> object dto </param>
    /// <returns>A generic response indicating the success or failure of the registration confirmation.</returns>
    public async Task<Util_GenericResponse<bool>>
    ConfirmRegistration
    (
        DTO_ConfirmRegistration confirmRegistrationDto
    )
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(confirmRegistrationDto.Email);

            if (user is null)
            {
                _logger.Log
                (
                   LogLevel.Information,
                   $"""
                        [Authentication Module]-[AUTH_Register Class]-[ConfirmRegistration Method] => 
                        [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user with email {confirmRegistrationDto.Email} doesn't exists.
                    """
                );

                return Util_GenericResponse<bool>.Response(false, false, "User doesn't exists", null, System.Net.HttpStatusCode.NotFound);
            }

            user.ModifiedAt = DateTime.Now;

            var verifyToken = await _userManager.ConfirmEmailAsync(user, confirmRegistrationDto.Token);


            if (!verifyToken.Succeeded)
            {
                _logger.Log
                (
                   LogLevel.Information,
                   $"""
                        [Authentication Module]-[AUTH_Register Class]-[ConfirmRegistration Method] => 
                        [IP] {await Util_GetIpAddres.GetLocation(_httpContextAccessor)} user with email {confirmRegistrationDto.Email}
                        tried to confirm the registration but the failed on the token validation. {@verifyToken.Errors.Select(x => x.Description)} 
                    """
                );

                return Util_GenericResponse<bool>.Response(false, false, "Something went wrong, try again.", null, System.Net.HttpStatusCode.NotFound);
            }

            return Util_GenericResponse<bool>.Response(true, true, "Email succsessfully validated", null, System.Net.HttpStatusCode.OK);

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AUTH_Register>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"Somewthing went wrong in [Authentication Module]-[AUTH_Register Class]-[ConfirmRegistration Method], " +
                $"user with  [Email] {confirmRegistrationDto.Email} tried to verify his registration.",
                false,
                _httpContextAccessor
            );
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
            return await Util_LogsHelper<bool, AUTH_Register>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"Somewthing went wrong in [Authentication Module] - [AssignUserToUserRole Method], " +
                $"user with [UserName] {userName}",
                false,
                _httpContextAccessor
            );
        }
    }
}