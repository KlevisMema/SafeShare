using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;

namespace SafeShare.ProxyApi.Container.Interfaces;

public interface IGroupManagmentProxyService
{
    Task<Util_GenericResponse<DTO_GroupsTypes>>
    GetGroupTypes
    (
        string jwtToken
    );
}