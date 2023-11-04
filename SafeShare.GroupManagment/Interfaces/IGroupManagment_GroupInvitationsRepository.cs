using SafeShare.Utilities.Responses;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.GroupManagment.Interfaces;

public interface IGroupManagment_GroupInvitationsRepository
{
    Task<Util_GenericResponse<List<DTO_RecivedInvitations>>>
    GetRecivedGroupsInvitations
    (
        Guid userId
    );

    Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        DTO_InvitationRequestActions accepInvitation
    );

    Task<Util_GenericResponse<List<DTO_SentInvitations>>>
     GetSentGroupInvitations
     (
        Guid userId
     );

    Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        DTO_InvitationRequestActions rejectInvitation
    );

    Task<Util_GenericResponse<bool>>
    SendInvitation
    (
       DTO_SendInvitationRequest sendInvitation
    );

    Task<Util_GenericResponse<bool>>
    DeleteSentInvitation
    (
        DTO_InvitationRequestActions deleteInvitation
    );
}