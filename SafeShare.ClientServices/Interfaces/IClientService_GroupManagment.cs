using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientUtilities.Responses;

namespace SafeShare.ClientServices.Interfaces
{
    public interface IClientService_GroupManagment
    {
        Task<ClientUtil_ApiResponse<ClientDto_GroupTypes>> GetGroupTypes();
    }
}