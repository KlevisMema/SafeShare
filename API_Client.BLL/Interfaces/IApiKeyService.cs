using SafeShare.Utilities.SafeShareApiKey.Helpers;
using SafeShare.DataTransormObject.SafeShareApiKey.ApiKey;

namespace API_Client.BLL.Interfaces;

public interface IApiKeyService
{
    Task<ServiceResponse<bool>> 
    DeleteApiKey
    (
        Guid id, 
        Guid clientId
    );


    Task<ServiceResponse<DTO_GetApiKey>> 
    GetApiKey
    (
        Guid id, 
        Guid clientId
    );


    Task<ServiceResponse<IEnumerable<DTO_GetApiKey>>> 
    GetApiKeys
    (
        Guid clientId
    );


    Task<ServiceResponse<DTO_GetApiKey>> 
    PostApiKey
    (
        DTO_CreateApiKey createDto
    );


    Task<ServiceResponse<bool>> 
    PutApiKey
    (
        Guid id, 
        Guid clientId, 
        DTO_UpdateApiKey updateDto
    );
}