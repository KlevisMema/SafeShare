/*
 * Account Management Class
 * 
 * This class encapsulates all the operations related to account management within the SafeShare application.
 * It provides methods for fetching user details, updating user information, deleting accounts, and changing user passwords.
 * Additionally, it utilizes the services provided by UserManager and the application's database context for various operations.
*/

using AutoMapper;
using Newtonsoft.Json.Linq;
using SendGrid.Helpers.Mail;
using SafeShare.Utilities.IP;
using SafeShare.Utilities.Log;
using SafeShare.Utilities.User;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Email;
using SafeShare.Utilities.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models;
using SafeShare.Utilities.Dependencies;
using SafeShare.DataAccessLayer.Context;
using SafeShare.UserManagment.Interfaces;
using Microsoft.Extensions.Configuration;
using SafeShare.DataTransormObject.Security;
using SafeShare.Utilities.ConfigurationSettings;
using SafeShare.Security.JwtSecurity.Interfaces;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.Security.JwtSecurity.Implementations;

namespace SafeShare.UserManagment.UserAccount;

/// <summary>
/// This class encapsulates all the operations related to account management within the SafeShare application
/// </summary>
public class AccountManagment : Util_BaseContextDependencies<ApplicationDbContext, AccountManagment>, IAccountManagment
{
    /// <summary>
    /// Provides the APIs for managing user in a persistence store.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;
    /// <summary>
    /// Provides settings config for reset password
    /// </summary>
    private readonly IOptions<Util_ResetPasswordSettings> _resetPasswordSettings;
    /// <summary>
    /// Provides settings config for activate account 
    /// </summary>
    private readonly IOptions<Util_ActivateAccountSettings> _activateAccountSettings;
    /// <summary>
    /// Provides settings config forchanging the email
    /// </summary>
    private readonly IOptions<Util_ChangeEmailAddressSettings> _changeEmailAddressSettings;
    /// <summary>
    /// Service to handle JWT token operations.
    /// </summary>
    private readonly ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser, DTO_Token> _jwtTokenService;
    /// <summary>
    /// Initializes a new instance of the AccountManagment class.
    /// </summary>
    /// <param name="resetPasswordSettings">Reset password settings</param>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="db">The application's database context instance.</param>
    /// <param name="changeEmailAddressSettings">Email change settings</param>
    /// <param name="activateAccountSettings">Activate account settings</param>
    /// <param name="mapper">The AutoMapper instance used for object-object mapping.</param>
    /// <param name="httpContextAccessor">Provides information about the HTTP request.</param>
    /// <param name="jwtTokenService">Provides services to handle jwt token operations.</param>
    /// <param name="userManager">UserManager instance to manage users in the persistence store.</param>
    public AccountManagment
    (
        ApplicationDbContext db,
        IMapper mapper,
        ILogger<AccountManagment> logger,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        IOptions<Util_ResetPasswordSettings> resetPasswordSettings,
        IOptions<Util_ActivateAccountSettings> activateAccountSettings,
        IOptions<Util_ChangeEmailAddressSettings> changeEmailAddressSettings,
        ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser, DTO_Token> jwtTokenService
    )
    :
    base
    (
        db,
        mapper,
        logger,
        httpContextAccessor
    )
    {
        _userManager = userManager;
        _resetPasswordSettings = resetPasswordSettings;
        _activateAccountSettings = activateAccountSettings;
        _changeEmailAddressSettings = changeEmailAddressSettings;
        _jwtTokenService = jwtTokenService;
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
        var getUserResult = await GetUserInfoMapped(id, null);

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
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [UserManagment Module]-[AccountManagment Class]-[UpdateUser Method] =>
                        [IP] {IP}
                        User with [ID] {id} doesn't exists
                        Dto {@DTO} and User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    id,
                    dtoUser,
                    getUser
                );

                return Util_GenericResponse<DTO_UserUpdatedInfo>.Response
                (
                    null,
                    false,
                    "User doesn't exists",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            getUser.ModifiedAt = DateTime.UtcNow;
            getUser.FullName = dtoUser.FullName;
            getUser.Birthday = dtoUser.Birthday;
            getUser.Gender = dtoUser.Gender;
            getUser.UserName = dtoUser.UserName;
            getUser.NormalizedUserName = dtoUser.UserName.ToUpper();
            getUser.PhoneNumber = dtoUser.PhoneNumber;
            getUser.RequireOTPDuringLogin = dtoUser.Enable2FA;

            await DeleteUserRefreshTokens(getUser.Id);

            var token = await GetToken(getUser);

            _logger.Log
            (
                LogLevel.Information,
                """
                     [UserManagment Module]-[AccountManagment Class]-[UpdateUser Method],
                     [IP] {IP}
                     User with [ID] : {id} just updated his data at {userModifiedAt}
                     Dto {@DTO} | User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                id,
                getUser.ModifiedAt,
                dtoUser,
                getUser
            );

            return await GetUserInfoMapped(id, token);
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_UserUpdatedInfo, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                     Somewthing went wrong in [UserManagment Module]-[AccountManagment Class]-[UpdateUser Method],
                     user with [ID] {id}.
                 """,
                null,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Deactivates the user based on the provided user ID.
    /// </summary>
    /// <param name="id">The ID of the user to deactivate.</param>
    /// <returns>A generic response indicating whether the user was successfully deactivated or not.</returns>
    public async Task<Util_GenericResponse<bool>>
    DeactivateAccount
    (
        Guid id,
        DTO_DeactivateAccount deactivateAccount
    )
    {
        try
        {

            var user = await GetApplicationUser(id);

            if (user is null || user.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [UserManagment Module]-[AccountManagment Class]-[DeactivateAccount Method] =>
                         [IP] {IP}
                         User with [ID] {id} doesn't exists.
                         DTO {@DTO} | User {@User} .
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    id,
                    deactivateAccount,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "User doesn't exists",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var identifyDeletionIsByTheUser = await _userManager.CheckPasswordAsync(user, deactivateAccount.Password);

            if (!identifyDeletionIsByTheUser)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [UserManagment Module]-[AccountManagment Class]-[DeactivateAccount Method] =>
                        User with [IP] {IP}
                        tried to delete user with [ID] {ID} doesn't exists
                        DTO {@DTO} | User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    id,
                    deactivateAccount,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Something went wrong, try again later.",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.Now;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [UserManagment Module]-[AccountManagment Class]-[DeactivateAccount Method] =>
                         [IP] {IP}
                         User with [ID] {ID} could not deactivate his profile due to errors.{@UpdateErrors}
                         DTO {@DTO} | User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    id,
                    updateResult,
                    deactivateAccount,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Something went wrong while deactivating the account",
                    updateResult.Errors.Select(x => x.Description).ToList(),
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            _logger.Log
            (
                LogLevel.Information,
                """
                    [UserManagment Module]-[AccountManagment Class]-[DeactivateAccount Method],
                    [IP] {IP}
                    User with [ID] : {ID} deactivated his account at {DeletedAt}
                    DTO {@DTO} | User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                id,
                user.DeletedAt,
                deactivateAccount,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "Your account was deactivated succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                     Somewthing went wrong in [UserManagment Module]-[AccountManagment Class]-[DeactivateAccount Method],
                     user with [ID] {id}".
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Reactivate the account request
    /// </summary>
    /// <param name="email"> The email of the user </param>
    /// <returns> A generic response indicating when the user activation request was successfully or not </returns>
    public async Task<Util_GenericResponse<bool>>
    ActivateAccountRequest
    (
        string email
    )
    {
        var user = await GetApplicationUserByEmail(email);

        if (user is null || user.Email is null)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                    [UserManagment Module]- [AccountManagment Class]-[ActivateAccountRequest Method] =>
                    User with [IP] {IP} and
                    [Email] {Email} tried to activate the account. User doesn't exists.
                    User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "User doesn't exists",
                null,
                System.Net.HttpStatusCode.NotFound
            );
        }

        if (!user.IsDeleted)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                     [UserManagment Module]- [AccountManagment Class]-[ActivateAccountRequest Method] =>
                     User with [IP] {IP} and
                     [Email] {Email} tried to activate the account. User account is already active.
                     User {@User}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "Your account is already active.",
                null,
                System.Net.HttpStatusCode.BadRequest
            );
        }

        try
        {
            var token = await _userManager.GenerateUserTokenAsync(user, "Default", _activateAccountSettings.Value.Reason);

            if (String.IsNullOrEmpty(token))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [UserManagment Module]- [AccountManagment Class]-[ActivateAccountRequest Method] =>
                         User with [IP] {IP} and
                         [Email] {Email} tried to activate the account. Token was not generated.
                         User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    email,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "There was an error, try again.",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var route = _activateAccountSettings.Value.Route.Replace("{token}", token).Replace("{email}", email);

            var sendEmailResult = await Util_Email.SendActivateAccountEmail(user.Email, user.FullName, route);

            if (!sendEmailResult.IsSuccessStatusCode)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [UserManagment Module]- [AccountManagment Class]-[ActivateAccountRequest Method] =>
                         User with [IP] {IP} and
                         [Email] {Email} tried to activate the account. Email was not sent.
                         EmailResult {@EmailResult} | User {User} ,
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    email,
                    await sendEmailResult.DeserializeResponseBodyAsync(sendEmailResult.Body),
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "There was an error in sending the email, try again.",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            _logger.Log
            (
                LogLevel.Information,
                """
                     [UserManagment Module]- [AccountManagment Class]-[ActivateAccountRequest Method] =>
                     User with [IP] {IP} and
                     [Email] {Email} tried to activate the account. Email was succsessfully sent.
                     User {@User} 
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "An email was sent to you for account reactivation",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                     Somewthing went wrong in [UserManagment Module] - [ActivateAccountRequest Method],   +
                     user with [Email] {email} tried to make a request to reactivate the account ,
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Confirms the account activation
    /// </summary>
    /// <param name="accountConfirmation"> The <see cref=DTO_ActivateAccountConfirmation""/> object </param>
    /// <returns> A generic response indicating if the users account was activated or not </returns>
    public async Task<Util_GenericResponse<bool>>
    ActivateAccountConfirmation
    (
        DTO_ActivateAccountConfirmation accountConfirmation
    )
    {
        var user = await GetApplicationUserByEmail(accountConfirmation.Email);

        if (user is null)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                    [UserManagment Module]- [AccountManagment Class]-[ActivateAccountConfirmation Method] =>
                    User with [IP] {IP} and
                    [Email] {Email} tried to confirm the activation of the account. User doesn't exists.
                    DTO {@DTO} | User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                accountConfirmation.Email,
                accountConfirmation,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "User doesn't exists",
                null,
                System.Net.HttpStatusCode.NotFound
            );
        }

        if (!user.IsDeleted)
        {
            _logger.Log
            (
               LogLevel.Error,
               """
                    [UserManagment Module]- [AccountManagment Class]-[ActivateAccountConfirmation Method] =>
                    User with [IP] {IP} and
                    [Email] {Email} tried to activate the account. User account is already active.
                    DTO {@DTO} | User {@User}.
                """,
               await Util_GetIpAddres.GetLocation(_httpContextAccessor),
               accountConfirmation.Email,
               accountConfirmation,
               user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "Your account is already active.",
                null,
                System.Net.HttpStatusCode.BadRequest
            );
        }

        try
        {
            var validToken = await _userManager.VerifyUserTokenAsync
            (
                user,
                "Default",
                _activateAccountSettings.Value.Reason,
                accountConfirmation.Token
            );

            if (!validToken)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [UserManagment Module]- [AccountManagment Class]-[ActivateAccountConfirmation Method] =>
                        User with [IP] {IP} and
                        [Email] {Email} tried to activate the account. Token is not valid.
                        DTO {@DTO} | User {@User} | Token {Token}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    accountConfirmation.Email,
                    accountConfirmation,
                    user,
                    accountConfirmation.Token
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Something went wrong with the account confirmation, try again",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            user.IsDeleted = false;
            user.CreatedAt = DateTime.Now;
            user.ModifiedAt = DateTime.Now;
            user.DeletedAt = null;

            var updateUserResult = await _userManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                _logger.Log
                (
                   LogLevel.Error,
                   """
                        [UserManagment Module]- [AccountManagment Class]-[ActivateAccountConfirmation Method] =>
                        User with [IP] {IP} and
                        [Email] {Email} tried to activate the account. Update user failed {@updateUserResult}
                        DTO {@DTO} | User {@User}.
                    """,
                   await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                   accountConfirmation.Email,
                   updateUserResult,
                   accountConfirmation,
                   user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Something went wrong with the account confirmation, try again",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            _logger.Log
            (
                LogLevel.Error,
                """
                    [UserManagment Module]- [AccountManagment Class]-[ActivateAccountConfirmation Method] =>
                    User with [IP] {IP} and
                    [Email] {Email} tried to activate the account. Account updated succsessfully
                    DTO {@DTO} | User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                accountConfirmation.Email,
                accountConfirmation,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "Your account was successfully activated.",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError
           (
               ex,
               _logger,
               $"""
                    Somewthing went wrong in [UserManagment Module] - [ActivateAccountConfirmation Method],
                    user with [Email] {accountConfirmation.Email} tried to confirm his reactivate the account request,
                """,
               false,
               _httpContextAccessor
           );
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
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [UserManagment Module]-[AccountManagment Class]-[ChangePassword Method] =>
                         [IP] {IP}
                         User with [ID] {ID} doesn't exists User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    id,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "User doesn't exists",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var updatePasswordResult = await _userManager.ChangePasswordAsync(user, updatePassword.OldPassword, updatePassword.ConfirmNewPassword);

            if (!updatePasswordResult.Succeeded)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [UserManagment Module]-[AccountManagment Class]-[ChangePassword Method] =>
                         [IP] {IP}
                         User with [ID] {ID} could not update his password due to errors.
                         {@updatePasswordResult.Errors} User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    id,
                    updatePasswordResult,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Something went wrong while trying to update the password.",
                    updatePasswordResult.Errors.Select(x => x.Description).ToList(),
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            user.ModifiedAt = DateTime.Now;
            await _userManager.UpdateAsync(user);

            _logger.Log
            (
                LogLevel.Information,
                """
                     [UserManagment Module]-[AccountManagment Class]-[ChangePassword Method] =>
                     [IP] {IP},
                     User with [ID] {ID} just changed his password. User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                id,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "Password updated successfully",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                """
                     Somewthing went wrong in [UserManagment Module]-[AccountManagment Class]-[ChangePassword Method],
                     user with [ID] {id} 
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Send an email to the user email with the link to reset his password.
    /// </summary>
    /// <param name="email"> The email of the user </param>
    /// <returns> A generic result indicating the result of the operation </returns>
    public async Task<Util_GenericResponse<bool>>
    ForgotPassword
    (
        string email
    )
    {
        var user = await GetApplicationUserByEmail(email);

        if (user is null || user.Email == null)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                     [UserManagment Module]-[AccountManagment Class]-[ForgotPassword Method] =>
                     [IP] {IP} user with email {Email} doesn't exists. User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "User doesn't exists",
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
                     [UserManagment Module]-[AccountManagment Class]-[ForgotPassword Method] =>
                     [IP] {IP} user with email {Email} doesn't exists. User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "User doesn't exists",
                null,
                System.Net.HttpStatusCode.NotFound
            );
        }

        try
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var route = _resetPasswordSettings.Value.Route.Replace("{token}", token).Replace("{email}", email);

            var sendEmailResult = await Util_Email.SendForgotPassordTokenEmail(user.Email, user.FullName, route);

            if (!sendEmailResult.IsSuccessStatusCode)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                         [UserManagment Module]-[AccountManagment Class]-[ForgotPassword Method] =>
                         [IP] {IP}
                         user with email {Email} doesn't exists. User {@User}.
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    email,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Something went wrong in email sending.",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            _logger.Log
            (
                LogLevel.Information,
                """
                     [UserManagment Module]-[AccountManagment Class]-[ForgotPassword Method] =>
                     [IP] {IP}
                     An email was sent to {Email} for password restore. User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "An email has been sent to you with the link to restore the password.",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [UserManagment Module]-[AccountManagment Class]-[ForgotPassword Method], 
                    user with [ID] {user.Id}.
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Reset the password of the user.
    /// </summary>
    /// <param name="resetPassword">The reset password object</param>
    /// <returns>A generic response indicating the result of the operation</returns>
    public async Task<Util_GenericResponse<bool>>
    ResetPassword
    (
        DTO_ResetPassword resetPassword
    )
    {
        var user = await GetApplicationUserByEmail(resetPassword.Email);

        if (user == null)
        {
            _logger.Log
            (
                LogLevel.Error,
                """
                     [UserManagment Module]-[AccountManagment Class]-[ResetPassword Method] =>
                     [IP] {IP}
                     user with email {Email} doesn't exists. User {@User} .
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                resetPassword.Email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "User doesn't exists",
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
                    [UserManagment Module]-[AccountManagment Class]-[ResetPassword Method] =>
                    [IP] {IP} user with email {Email} doesn't exists.
                    User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                resetPassword.Email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                false,
                false,
                "User doesn't exists",
                null,
                System.Net.HttpStatusCode.NotFound
            );
        }

        try
        {
            var changePasswordResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                _logger.Log
               (
                    LogLevel.Error,
                    """
                         [UserManagment Module]-[AccountManagment Class]-[ResetPassword Method] =>
                         [IP] {IP} user with email {Email} tried to
                         reset the password but failed : {@Errors}. User {@User} .
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    resetPassword.Email,
                    changePasswordResult.Errors,
                    user
               );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Password was not restored",
                    changePasswordResult.Errors.Select(x => x.Description).ToList(),
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            _logger.Log
            (
                LogLevel.Information,
                """
                     [UserManagment Module]-[AccountManagment Class]-[ResetPassword Method] => 
                     [IP] {IP} user with email {Email} succsessfully
                     reset the password. User {@User}.
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                resetPassword.Email,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "Password was succsessfully restored",
                changePasswordResult.Errors.Select(x => x.Description).ToList(),
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [UserManagment Module]-[AccountManagment Class]-[ResetPassword Method], 
                    user with [ID] {user.Id}
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Request for changing the email
    /// </summary>
    /// <param name="newEmailAddressDto">The <see cref="DTO_ChangeEmailAddressRequest"/> object dto </param>
    /// <returns>A generic response indicating the result of the operation</returns>
    public async Task<Util_GenericResponse<bool>>
    RequestChangeEmailAddress
    (
        DTO_ChangeEmailAddressRequest newEmailAddressDto
    )
    {
        var userId = Util_FindUserIdFromToken.UserId(_httpContextAccessor);

        try
        {
            var user = await GetApplicationUser(Guid.Parse(userId));

            if (user == null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [UserManagment Module]-[AccountManagment Class]-[RequestChangeEmailAddress Method] =>
                        [IP] {IP}
                        user with email {CurrentEmailAddress} doesn't exists. DTO {@DTO} | User {@User}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    newEmailAddressDto.CurrentEmailAddress,
                    newEmailAddressDto,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "User doesn't exists",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            if (user.Email != newEmailAddressDto.CurrentEmailAddress)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [UserManagment Module]-[AccountManagment Class]-[ChangeEmailAddress Method] => 
                        [IP] {IP} 
                        user with email {CurrentEmailAddress} is not correct. DTO {@DTO} | User {@User}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    newEmailAddressDto.CurrentEmailAddress,
                    newEmailAddressDto,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Your old email address is incorrect",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            if (_userManager.Users.Any(x => x.Email == newEmailAddressDto.ConfirmNewEmailAddress))
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [UserManagment Module]-[AccountManagment Class]-[RequestChangeEmailAddress Method] => 
                        [IP] {IP} user with email {CurrentEmailAddress}
                        tried to change email to an email that exists in the database.
                        DTO {@DTO} | User {@User}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    newEmailAddressDto.CurrentEmailAddress,
                    newEmailAddressDto,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Email is taken",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            var tokenForEmailConfirmation = await _userManager.GenerateChangeEmailTokenAsync(user, newEmailAddressDto.ConfirmNewEmailAddress);

            var route = _changeEmailAddressSettings.Value.Route.Replace("{token}", tokenForEmailConfirmation).Replace("{email}", newEmailAddressDto.ConfirmNewEmailAddress);

            var sendEmailResult = await Util_Email.SendEmailForEmailConfirmation(newEmailAddressDto.ConfirmNewEmailAddress, route, user.FullName);

            if (!sendEmailResult.IsSuccessStatusCode)
            {
                _logger.Log
                (
                   LogLevel.Error,
                   """
                        [UserManagment Module]-[AccountManagment Class]-[RequestChangeEmailAddress Method] => 
                        [IP] {IP} user with email {CurrentEmailAddress} 
                        tried to change his email address and failed {@sendEmailResult} | {@sendEmailResultBody}.
                        DTO {@DTO} | User {@User}
                    """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    newEmailAddressDto.CurrentEmailAddress,
                    await sendEmailResult.DeserializeResponseBodyAsync(sendEmailResult.Body),
                    sendEmailResult.Body.ReadAsStringAsync().Result,
                    newEmailAddressDto,
                    user
                );

                return Util_GenericResponse<bool>.Response
                (
                    false,
                    false,
                    "Your old email address is incorrect",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            _logger.Log
            (
                LogLevel.Information,
                """
                    [UserManagment Module]-[AccountManagment Class]-[RequestChangeEmailAddress Method] => 
                    [IP] {IP} user with email {CurrentEmailAddress} 
                    changed his email address succsessfully.
                    DTO {@DTO} | User {@User}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                newEmailAddressDto.CurrentEmailAddress,
                newEmailAddressDto,
                user
            );

            return Util_GenericResponse<bool>.Response
            (
                true,
                true,
                "An email has been sent to your new email.",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<bool, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [UserManagment Module]-[ChangeEmailAddress Class]-[RequestChangeEmailAddress Method],
                    user with [ID] {userId} and [Email] {newEmailAddressDto.CurrentEmailAddress}
                 """,
                false,
                _httpContextAccessor
            );
        }
    }
    /// <summary>
    /// Confirms the request of changing the email.
    /// </summary>
    /// <param name="changeEmailAddressConfirmDto"> The <see cref="DTO_ChangeEmailAddressRequestConfirm"/> object dto </param>
    /// <returns>A generic response indicating the result of the operation</returns>
    public async Task<Util_GenericResponse<DTO_Token>>
    ConfirmChangeEmailAddressRequest
    (
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirmDto
    )
    {
        var userId = Util_FindUserIdFromToken.UserId(_httpContextAccessor);

        try
        {
            var user = await GetApplicationUser(Guid.Parse(userId));

            if (user == null)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                        [UserManagment Module]-[AccountManagment Class]-[ConfirmRequestChangeEmailAddress Method] => 
                        [IP] {IP} user with email {EmailAddress}
                        tried to confirm the request for changing the email but the id was not extracted from the token.
                        DTO {@DTO} | User {@User}
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    changeEmailAddressConfirmDto.EmailAddress,
                    changeEmailAddressConfirmDto,
                    user
                );

                return Util_GenericResponse<DTO_Token>.Response
                (
                    null,
                    false,
                    "Something went wrong, please log in and try again.",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            user.ModifiedAt = DateTime.Now;

            var confirmTokenResult = await _userManager.ChangeEmailAsync(user, changeEmailAddressConfirmDto.EmailAddress, changeEmailAddressConfirmDto.Token);

            if (!confirmTokenResult.Succeeded)
            {
                _logger.Log
                (
                   LogLevel.Error,
                   """
                        [UserManagment Module]-[AccountManagment Class]-[ConfirmRequestChangeEmailAddress Method] => 
                        [IP] {IP} user with email {EmailAddress}
                        tried to confirm the request for changing the email but confirmTokenResult failed with {@ErrorsDescription}.
                        DTO {@DTO} | User {@User}
                    """,
                   await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                   changeEmailAddressConfirmDto.EmailAddress,
                   confirmTokenResult.Errors.Select(x => x.Description),
                   changeEmailAddressConfirmDto,
                   user
                );

                return Util_GenericResponse<DTO_Token>.Response
                (
                    null,
                    false,
                    "Something went wrong, your email was not verified succsessfully",
                    null,
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            await DeleteUserRefreshTokens(user.Id);

            var token = await GetToken(user);

            _logger.Log
            (
                LogLevel.Information,
                """
                    [UserManagment Module]-[AccountManagment Class]-[ConfirmRequestChangeEmailAddress Method] => 
                    [IP] {IP} user with email {EmailAddress}
                    tried to confirm the request for changing the email but confirmTokenResult failed with {@ErrorsDescription}.
                    DTO {@DTO} | User {@User}
                 """,
                await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                changeEmailAddressConfirmDto.EmailAddress,
                confirmTokenResult.Errors.Select(x => x.Description),
                changeEmailAddressConfirmDto,
                user
            );

            return Util_GenericResponse<DTO_Token>.Response
            (
                token,
                true,
                "Your email was succsessfully changed.",
                null,
                System.Net.HttpStatusCode.OK
            );

        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_Token, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [UserManagment Module]-[ChangeEmailAddress Class]-[ConfirmRequestChangeEmailAddress Method],
                    user with [ID] {userId} and [Email] {changeEmailAddressConfirmDto.EmailAddress} tried to confirm the request to confirm the email.
                 """,
                null,
                _httpContextAccessor
            );
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
        Guid id,
        DTO_Token? userToken
    )
    {
        try
        {
            var user = await GetApplicationUser(id);

            if (user is null || user.IsDeleted)
            {
                _logger.Log
                (
                    LogLevel.Error,
                    """
                          [UserManagment Module]-[AccountManagment Class]-[GetUserInfoMapped Method] =>
                          [IP] {IP}
                          User with [ID] {ID} doesn't exists 
                     """,
                    await Util_GetIpAddres.GetLocation(_httpContextAccessor),
                    id
                );

                return Util_GenericResponse<DTO_UserUpdatedInfo>.Response
                (
                    null,
                    false,
                    "User doesn't exists",
                    null,
                    System.Net.HttpStatusCode.NotFound
                );
            }

            var userInfoMapped = _mapper.Map<DTO_UserUpdatedInfo>(user);
            userInfoMapped.UserToken = userToken;

            return Util_GenericResponse<DTO_UserUpdatedInfo>.Response
            (
                userInfoMapped,
                true,
                "User retrieved succsessfully",
                null,
                System.Net.HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            return await Util_LogsHelper<DTO_UserUpdatedInfo, AccountManagment>.ReturnInternalServerError
            (
                ex,
                _logger,
                $"""
                    Somewthing went wrong in [UserManagment Module]-[AccountManagment Class]-[GetUserInfoMapped Method], 
                    user with [ID] {id}
                 """,
                null,
                _httpContextAccessor
            );
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
    /// <summary>
    /// Fetches the ApplicationUser from the persistence store based on the provided user email.
    /// </summary>
    /// <param name="email">The email of the user to be fetched.</param>
    /// <returns>The ApplicationUser if found, null otherwise.</returns>
    private async Task<ApplicationUser?>
    GetApplicationUserByEmail
    (
        string email
    )
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user;
    }
    /// <summary>
    /// Get the jwt token.
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
        var token = await _jwtTokenService.CreateToken(userDto);

        return token;
    }
    /// <summary>
    /// Deletes users refresh tokens 
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>A async task</returns>
    private async Task
    DeleteUserRefreshTokens
    (
        string userId
    )
    {
        var userRefreshTokens = await _db.RefreshTokens.Include(x => x.User)
                                                         .Where(x => x.UserId == userId)
                                                         .ToArrayAsync();

        _db.RefreshTokens.RemoveRange(userRefreshTokens);

        await _db.SaveChangesAsync();
    }
}