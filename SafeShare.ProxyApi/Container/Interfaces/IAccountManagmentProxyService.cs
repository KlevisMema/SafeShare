using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.ProxyApi.Container.Interfaces;

public interface IAccountManagmentProxyService
{
    Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    GetUser
    (
        string userId,
        string jwtToken
    );

    Task<Util_GenericResponse<DTO_UserUpdatedInfo>>
    UpdateUser
    (
        string userId,
        string jwtToken,
        DTO_UserInfo userInfo
    );

    Task<Util_GenericResponse<bool>>
    ChangePassword
    (
        string userId,
        string jwtToken,
        DTO_UserChangePassword changePassword
    );

    Task<Util_GenericResponse<bool>>
    DeactivateAccount
    (
        string userId,
        string jwtToken,
        DTO_DeactivateAccount deactivateAccount
    );

    Task<Util_GenericResponse<bool>>
    ActivateAccountRequest
    (
        string email
    );

    Task<Util_GenericResponse<bool>>
    ActivateAccountRequestConfirmation
    (
        DTO_ActivateAccountConfirmation activateAccountConfirmationDto
    );

    Task<Util_GenericResponse<bool>>
    ForgotPassword
    (
        [FromForm] DTO_ForgotPassword forgotPassword
    );

    Task<Util_GenericResponse<bool>>
    ResetPassword
    (
        DTO_ResetPassword resetPassword
    );

    Task<Util_GenericResponse<bool>>
    RequestChangeEmail
    (
        string userId,
        string jwtToken,
        DTO_ChangeEmailAddressRequest emailAddress
    );

    Task<Util_GenericResponse<bool>>
    ConfirmChangeEmailAddressRequest
    (
        string userId,
        string jwtToken,
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirmDto
    );

    Task<Util_GenericResponse<List<DTO_UserSearched>>>
    SearchUserByUserName
    (
        string userId,
        string jwtToken,
        string userName
    );
}