using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.ProxyApi.Container.Interfaces;

public interface IGroupManagmentProxyService
{
    Task<Util_GenericResponse<DTO_GroupsTypes>>
    GetGroupTypes
    (
        string jwtToken
    );

    Task<Util_GenericResponse<DTO_GroupDetails>>
    GetGroupDetails
    (
        string userId,
        string jwtToken,
        Guid groupId
    );

    Task<Util_GenericResponse<DTO_GroupType>>
    CreateGroup
    (
         string userId,
        string jwtToken,
        DTO_CreateGroup createGroup
    );

    Task<Util_GenericResponse<DTO_GroupType>>
    EditGroup
    (
        string userId,
        string jwtToken,
        Guid groupId,
        DTO_EditGroup editGroup
    );

    Task<Util_GenericResponse<bool>>
    DeleteGroup
    (
        string userId,
        string jwtToken,
        Guid groupId
    );

    Task<Util_GenericResponse<List<DTO_RecivedInvitations>>>
    GetGroupsInvitations
    (
        string userId,
        string jwtToken
    );

    Task<Util_GenericResponse<List<DTO_SentInvitations>>>
    GetSentGroupInvitations
    (
        string userId,
        string jwtToken
    );

    Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        string userId,
        string jwtToken,
        DTO_SendInvitationRequest dTO_SendInvitation
    );

    Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        string userId,
        string jwtToken,
        DTO_InvitationRequestActions acceptInvitationRequest
    );

    Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        string userId,
        string jwtToken,
        DTO_InvitationRequestActions rejectInvitationRequest
    );

    Task<Util_GenericResponse<bool>>
    DeleteInvitation
    (
        string userId,
        string jwtToken,
        DTO_InvitationRequestActions deleteInvitationRequest
    );
}