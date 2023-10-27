/*
 * Account Management Class
 * 
 * This class encapsulates all the operations related to account management within the SafeShare application.
 * It provides methods for fetching user details, updating user information, deleting accounts, and changing user passwords.
 * Additionally, it utilizes the services provided by UserManager and the application's database context for various operations.
*/

using AutoMapper;
using SafeShare.Utilities.IP;
using SafeShare.Utilities.Log;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Services;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.UserManagment.Interfaces;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.UserManagment.UserAccount;

/// <summary>
/// This class encapsulates all the operations related to account management within the SafeShare application
/// </summary>
public class AccountManagment : Util_BaseDependencies<AccountManagment>, IAccountManagment
{
    /// <summary>
    /// Provides methods to interact with the application's database.
    /// </summary>
    private readonly ApplicationDbContext _db;
    /// <summary>
    /// Provides the APIs for managing user in a persistence store.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;
    /// <summary>
    /// Initializes a new instance of the AccountManagment class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance used for object-object mapping.</param>
    /// <param name="db">The application's database context instance.</param>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="userManager">UserManager instance to manage users in the persistence store.</param>
    /// <param name="httpContextAccessor">Provides information about the HTTP request.</param>
    public AccountManagment
    (
        IMapper mapper,
        ApplicationDbContext db,
        ILogger<AccountManagment> logger,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor
    )
    :
    base
    (
        mapper,
        logger,
        httpContextAccessor
    )
    {
        _userManager = userManager;
        _db = db;
    }
    /// <summary>
    /// Retrieves the user details based on the provided user ID.
    /// </summary>
    /// <param name="id">The ID of the user to be retrieved.</param>
    /// <returns>A response containing user details.</returns>
    public async Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUser
    (
        Guid id
    )
    {
        var getUserResult = await GetUserInfoMapped(id);

        return getUserResult;
    }
    /// <summary>
    /// Updates user details based on the provided user ID and updated user information.
    /// </summary>
    /// <param name="id">The ID of the user to be updated.</param>
    /// <param name="dtoUser">The updated user information.</param>
    /// <returns>A response indicating the result of the update operation.</returns>
    public async Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    UpdateUser
    (
        Guid id,
        DTO_UserInfo dtoUser
    )
    {
        try
        {
            var getUser = await GetApplicationUser(id);

            if (getUser is null || getUser.IsDeleted)
            {
                _logger.Log(LogLevel.Information, $"[UpdateUser Method] => User with [ID] [{id}] doesn't exists");
                return Util_GenericResponse<DTO_UserUpdatedInfo>.Response(null, false, "User doesn't exists", null, System.Net.HttpStatusCode.NotFound);
            }

            var newValuesOfApplicationUser = _mapper.Map<ApplicationUser>(dtoUser);

            newValuesOfApplicationUser.Id = id.ToString();
            newValuesOfApplicationUser.PasswordHash = getUser.PasswordHash;
            newValuesOfApplicationUser.CreatedAt = getUser.CreatedAt;

            _db.Entry(getUser).CurrentValues.SetValues(newValuesOfApplicationUser);
            await _db.SaveChangesAsync();   

            _logger.Log(LogLevel.Information, $"[UserManagment Module] [UpdateUser Method], User with => [IP] [{await Util_GetIpAddres.GetLocation(_httpContextAccessor)}] | [ID] : [{id}] just updated his data at {newValuesOfApplicationUser.ModifiedAt}");

            return await GetUserInfoMapped(id);
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_UserUpdatedInfo, AccountManagment>.ReturnInternalServerError(ex, _logger, $"Somewthing went wrong in [UserManagment Module] - [UpdateUser Method], user with [ID] {id}", null, _httpContextAccessor);
        }
    }
    /// <summary>
    /// Deletes the user based on the provided user ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>A generic response indicating whether the user was successfully deleted or not.</returns>
    public async Task<Util_GenericResponse<bool>>
    DeleteUser
    (
        Guid id
    )
    {
        try
        {
            var user = await GetApplicationUser(id);

            if (user is null || user.IsDeleted)
            {
                _logger.Log(LogLevel.Information, $"[DeleteUser Method] => User with [ID] [{id}] doesn't exists");
                return Util_GenericResponse<bool>.Response(false, false, "User doesn't exists", null, System.Net.HttpStatusCode.NotFound);
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.Now;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.Log(LogLevel.Information, $"[DeleteUser Method] => User with [ID] [{id}] could not delete his profile due to errors.{@updateResult.Errors}", updateResult);
                return Util_GenericResponse<bool>.Response(false, false, "Something went wrong while deleting the account", updateResult.Errors.Select(x => x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
            }

            _logger.Log(LogLevel.Information, $"[UserManagment Module] [UpdateUser Method], User with => [IP] [{await Util_GetIpAddres.GetLocation(_httpContextAccessor)}] | [ID] : [{id}] deleted his account at {user.DeletedAt}");

            return Util_GenericResponse<bool>.Response(true, true, "Your account was deleted succsessfully", null, System.Net.HttpStatusCode.OK);

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError(ex, _logger, $"Somewthing went wrong in [UserManagment Module] - [DeleteUser Method], user with [ID] {id}", false, _httpContextAccessor);
        }
    }
    /// <summary>
    /// Changes the password of a user based on the provided user ID and password details.
    /// </summary>
    /// <param name="id">The ID of the user whose password needs to be changed.</param>
    /// <param name="updatePassword">The details containing the old and new password information.</param>
    /// <returns>A generic response indicating whether the password was successfully changed or not.</returns>
    public async Task<Util_GenericResponse<bool>>
    ChangePassword
    (
        Guid id,
        DTO_UserChangePassword updatePassword
    )
    {
        try
        {
            var user = await GetApplicationUser(id);

            if (user is null || user.IsDeleted)
            {
                _logger.Log(LogLevel.Information, $"[UpdatePassword Method] => User with [ID] [{id}] doesn't exists");
                return Util_GenericResponse<bool>.Response(false, false, "User doesn't exists", null, System.Net.HttpStatusCode.NotFound);
            }

            var updatePasswordResult = await _userManager.ChangePasswordAsync(user, updatePassword.OldPassword, updatePassword.ConfirmNewPassword);

            if (!updatePasswordResult.Succeeded)
            {
                _logger.Log(LogLevel.Information, $"[UpdatePassword Method] => User with [ID] [{id}] could not update his password due to errors. {@updatePasswordResult.Errors}", updatePasswordResult);
                return Util_GenericResponse<bool>.Response(false, false, "Something went wrong while trying to update the password.", updatePasswordResult.Errors.Select(x => x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
            }

            return Util_GenericResponse<bool>.Response(true, true, "Password updated successfully", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError(ex, _logger, $"Somewthing went wrong in [UserManagment Module] - [ChangePassword Method], user with [ID] {id}", false, _httpContextAccessor);
        }
    }
    /// <summary>
    /// Retrieves the user details mapped to DTO based on the provided user ID.
    /// </summary>
    /// <param name="id">The ID of the user to be retrieved.</param>
    /// <returns>A response containing user details in DTO format.</returns>
    private async Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUserInfoMapped
    (
        Guid id
    )
    {
        try
        {
            var user = await GetApplicationUser(id);

            if (user is null || user.IsDeleted)
            {
                _logger.Log(LogLevel.Information, $"[GetUserInfoMapped Method] => User with [ID] [{id}] doesn't exists");
                return Util_GenericResponse<DTO_UserUpdatedInfo>.Response(null, false, "User doesn't exists", null, System.Net.HttpStatusCode.NotFound);
            }

            return Util_GenericResponse<DTO_UserUpdatedInfo>.Response(_mapper.Map<DTO_UserUpdatedInfo>(user), true, "User retrieved succsessfully", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_UserUpdatedInfo, AccountManagment>.ReturnInternalServerError(ex, _logger, $"Somewthing went wrong in [UserManagment Module] - [GetUserInfoMapped Method], user with [ID] {id}", null, _httpContextAccessor);
        }
    }
    /// <summary>
    /// Fetches the ApplicationUser from the persistence store based on the provided user ID.
    /// </summary>
    /// <param name="id">The ID of the user to be fetched.</param>
    /// <returns>The ApplicationUser if found, null otherwise.</returns>
    private async Task<ApplicationUser?>
    GetApplicationUser
    (
        Guid id
    )
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        return user;
    }
}