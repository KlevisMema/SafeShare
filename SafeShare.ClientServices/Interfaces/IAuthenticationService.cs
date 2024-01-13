using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientUtilities.Responses;

namespace SafeShare.ClientServices.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ClientUtil_ApiResponse<bool>>
        RegisterUser
        (
            ClientDto_Register register
        );

        Task<ClientUtil_ApiResponse<ClientDto_LoginResult>>
        LogInUser
        (
            ClientDto_Login login
        );

        Task
        LogoutUser
        (
            Guid userId
        );

        Task<ClientUtil_ApiResponse<bool>>
        ConfirmUserRegistration
        (
            ClientDto_ConfirmRegistration confirmRegistration
        );

        Task<ClientUtil_ApiResponse<bool>>
        ReConfirmRegistrationRequest
        (
            ClientDto_ReConfirmRegistration ConfirmRegistration
        );

        Task<ClientUtil_ApiResponse<ClientDto_LoginResult>>
        ConfirmLogin2FA
        (
            ClientDto_2FA TwoFA
        );
    }
}