using SafeShare.Utilities.SafeShareApiKey.Helpers;

namespace API_Client.BLL.Interfaces;

public interface IAuthenticationService
{
    Task<ServiceResponse<string>>
    Login
    (
        string userName,
        string password
    );
}