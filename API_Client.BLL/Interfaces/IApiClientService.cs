using SafeShare.Utilities.SafeShareApiKey.Helpers;
using SafeShare.DataTransormObject.SafeShareApiKey.User;

namespace API_Client.BLL.Interfaces
{
    public interface IApiClientService
    {
        Task<ServiceResponse<DTO_GetApiClient>> 
        CreateApiClient
        (
            DTO_CreateApiClient createDto
        );


        Task<ServiceResponse<bool>> 
        DeleteApiClient
        (
            Guid id
        );


        Task<ServiceResponse<DTO_GetApiClient>> 
        GetApiClient
        (
            Guid id
        );


        Task<ServiceResponse<DTO_GetApiClient>> 
        UpdateApiClient
        (
            Guid id, 
            DTO_UpdateApiClient updateDto
        );
    }
}