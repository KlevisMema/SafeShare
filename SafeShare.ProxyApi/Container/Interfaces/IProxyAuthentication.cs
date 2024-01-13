using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;

namespace SafeShare.ProxyApi.Container.Interfaces;

public interface IProxyAuthentication
{
    Task<Util_GenericResponse<bool>>
    RegisterUser
    (
        DTO_Register register
    );

    Task<Util_GenericResponse<bool>>
    ConfirmRegistration
    (
         DTO_ConfirmRegistration confirmRegistrationDto
    );

    Task<Util_GenericResponse<bool>>
    ReConfirmRegistrationRequest
    (
        DTO_ReConfirmRegistration ReConfirmRegistration
    );

    Task<Tuple<Util_GenericResponse<DTO_LoginResult>, HttpResponseMessage>>
    LogIn
    (
        DTO_Login loginDto
    );

    Task<Tuple<Util_GenericResponse<DTO_LoginResult>, HttpResponseMessage>>
    ConfirmLogin2FA
    (
        Guid userId,
        string jwtToken,
        DTO_ConfirmLogin confirmLogin
    );

    Task<HttpResponseMessage>
    LogoutUser
    (
        Guid userId,
        string jwtToken
    );

    Task<Tuple<Util_GenericResponse<DTO_Token>, HttpResponseMessage>>
    RefreshToken
    (
        string jwtToken,
        string refreshToken,
        string refreshTokenId
    );
}