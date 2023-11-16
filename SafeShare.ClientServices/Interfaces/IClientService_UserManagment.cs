using SafeShare.ClientUtilities.Responses;

namespace SafeShare.ClientServices.Interfaces;

public interface IClientService_UserManagment
{
    Task<ClientUtil_ApiResponse<bool>> CallTheApi();
}