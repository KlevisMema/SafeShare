using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.ProxyApi.Container.Interfaces;

public interface IGroupManagmentProxyService
{
    Task<Tuple<Util_GenericResponse<DTO_GroupsTypes>, HttpResponseMessage>>
    GetGroupTypes
    (
        string userId,
        string userIp,
        string jwtToken
    );

    Task<Util_GenericResponse<DTO_GroupDetails>>
    GetGroupDetails
    (
        string userId,
        string userIp,
        string jwtToken,
        Guid groupId
    );

    Task<Util_GenericResponse<DTO_GroupType>>
    CreateGroup
    (
        string userId,
        string userIP,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_CreateGroup createGroup
    );

    Task<Util_GenericResponse<DTO_GroupType>>
    EditGroup
    (
        string userId,
        string userIp,
        string fogeryToken,
        string aspNetForgeryToken,
        string jwtToken,
        Guid groupId,
        DTO_EditGroup editGroup
    );

    Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        string userId,
        string userIp,
        string fogeryToken,
        string aspNetForgeryToken,
        string jwtToken,
        Guid groupId,
        List<string> membersId,
        string groupName
    );

    Task<Util_GenericResponse<List<DTO_RecivedInvitations>>>
    GetGroupsInvitations
    (
        string userId,
        string userIp,
        string jwtToken
    );

    Task<Util_GenericResponse<List<DTO_SentInvitations>>>
    GetSentGroupInvitations
    (
        string userId,
        string userIp,
        string jwtToken
    );

    Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_SendInvitationRequest dTO_SendInvitation
    );

    Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_InvitationRequestActions acceptInvitationRequest
    );

    Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_InvitationRequestActions rejectInvitationRequest
    );

    Task<Util_GenericResponse<bool>>
    DeleteInvitation
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        DTO_InvitationRequestActions deleteInvitationRequest
    );

    Task<Util_GenericResponse<bool>>
    DeleteUsersFromGroup
    (
        string userId,
        string userIp,
        string jwtToken,
        string fogeryToken,
        string aspNetForgeryToken,
        Guid groupId,
        List<DTO_UsersGroupDetails> UsersToRemoveFromGroup
    );
}