using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientUtilities.Responses;

namespace SafeShare.ClientServices.Interfaces
{
    public interface IAuthenticationService
    {
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
    }
}