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

public class AccountManagment : Util_BaseDependencies<AccountManagment>, IAccountManagment
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

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

    public async Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUser
    (
        Guid id
    )
    {
        var getUserResult = await GetUserInfoMapped(id);

        return getUserResult;
    }

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