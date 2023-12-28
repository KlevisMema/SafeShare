using SafeShare.ClientUtilities.Responses;
using SafeShare.ClientDTO.AccountManagment;

namespace SafeShare.ClientServices.Interfaces;

public interface IClientService_UserManagment
{
    Task<ClientUtil_ApiResponse<bool>> 
    ActivateAccountRequest();
    
    
    Task<ClientUtil_ApiResponse<bool>> 
    ActivateAccountRequestConfirmation
    (
        ClientDto_ActivateAccountConfirmation activateAccountConfirmation
    );


    Task<ClientUtil_ApiResponse<bool>> 
    ChangePassword
    (
        ClientDto_UserChangePassword userChangePassword
    );
    
    
    Task<ClientUtil_ApiResponse<bool>> 
    ConfirmChangeEmailAddressRequest
    (
        ClientDto_ChangeEmailAddressRequestConfirm changeEmailAddressRequestConfirm
    );
    
    
    Task<ClientUtil_ApiResponse<bool>> 
    DeactivateAccount
    (
        ClientDto_DeactivateAccount deactivateAccount
    );
    
    
    Task<ClientUtil_ApiResponse<bool>> 
    ForgotPassword
    (
        ClientDto_ForgotPassword forgotPassword
    );
    
    
    Task<ClientUtil_ApiResponse<ClientDto_UserInfo>> 
    GetUser();
    
    
    Task<ClientUtil_ApiResponse<bool>> 
    RequestChangeEmail
    (
        ClientDto_ChangeEmailAddressRequest changeEmailAddressRequest
    );
    
    
    Task<ClientUtil_ApiResponse<bool>> 
    ResetPassword
    (
        ClientDto_ResetPassword resetPassword
    );
    
    
    Task<ClientUtil_ApiResponse<List<ClientDto_UserSearched>>> 
    SearchUserByUserName
    (
        string userName
    );
    
    
    Task<ClientUtil_ApiResponse<ClientDto_UserInfo>> 
    UpdateUser
    (
        ClientDto_UpdateUser clientDto_UpdateUser
    );
}