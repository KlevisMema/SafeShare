using SafeShare.ClientDTO.AccountManagment;
using SafeShare.ClientUtilities.Responses;

namespace SafeShare.ClientServices.Interfaces;

public interface IClientService_UserManagment
{
    Task<ClientUtil_ApiResponse<bool>>
    ForgotPassword
    (
        ClientDto_ForgotPassword forgotPassword
    );

    Task<ClientUtil_ApiResponse<bool>>
    ResetPassword
    (
        ClientDto_ResetPassword resetPassword
    );

    Task<ClientUtil_ApiResponse<bool>>
    CallTheApi();
}