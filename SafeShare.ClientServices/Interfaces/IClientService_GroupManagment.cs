using SafeShare.ClientDTO.GroupManagment;
using SafeShare.ClientUtilities.Responses;

namespace SafeShare.ClientServices.GroupManagment;

public interface IClientService_GroupManagment
{
    Task<ClientUtil_ApiResponse<bool>>
    AcceptInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    );

    Task<ClientUtil_ApiResponse<ClientDto_GroupType>>
    CreateGroup
    (
        ClientDto_CreateGroup createGroup
    );

    Task<ClientUtil_ApiResponse<bool>>
    DeleteGroup
    (
        Guid groupId
    );

    Task<ClientUtil_ApiResponse<bool>>
    DeleteInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    );

    Task<ClientUtil_ApiResponse<ClientDto_GroupType>>
    EditGroup
    (
        Guid groupId,
        ClientDto_EditGroup editGroup
    );

    Task<ClientUtil_ApiResponse<ClientDto_GroupDetails>>
    GetGroupDetails
    (
        Guid groupId
    );

    Task<ClientUtil_ApiResponse<List<ClientDto_RecivedInvitations>>>
    GetGroupsInvitations();

    Task<ClientUtil_ApiResponse<ClientDto_GroupTypes>>
    GetGroupTypes();

    Task<ClientUtil_ApiResponse<List<ClientDto_SentInvitations>>>
    GetSentGroupInvitations();

    Task<ClientUtil_ApiResponse<bool>>
    RejectInvitation
    (
        ClientDto_InvitationRequestActions invitationRequestActions
    );

    Task<ClientUtil_ApiResponse<bool>>
    SendInvitation
    (
        ClientDto_SendInvitationRequest sendInvitationRequest
    );

    Task<ClientUtil_ApiResponse<bool>>
    DeleteUsersFromGroup
    (
        Guid groupId,
        List<ClientDto_UsersGroupDetails> usersOfTheGroup
    );
}