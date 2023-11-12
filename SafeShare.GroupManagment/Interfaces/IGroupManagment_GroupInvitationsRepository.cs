/* 
 * Defines the contract for a service responsible for group invitations operations within the Group Management module. 
 * It provides functionalities like receiving, sending, accepting, and rejecting group invitations.
 */

using SafeShare.Utilities.Responses;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.GroupManagment.Interfaces;

/// <summary>
/// Defines the contract for a repository managing group invitations in the Group Management module.
/// It includes functionalities for handling various aspects of group invitations such as receiving, 
/// sending, accepting, and rejecting them.
/// </summary>  
public interface IGroupManagment_GroupInvitationsRepository
{
    /// <summary>
    /// Retrieves a list of received group invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose received invitations are to be fetched.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with a list of received invitations.</returns>
    Task<Util_GenericResponse<List<DTO_RecivedInvitations>>>
    GetRecivedGroupsInvitations
    (
        Guid userId
    );
    /// <summary>
    /// Retrieves a list of sent group invitations for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose sent invitations are to be fetched.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response with a list of sent invitations.</returns>
    Task<Util_GenericResponse<List<DTO_SentInvitations>>>
    GetSentGroupInvitations
    (
        Guid userId
    );
    /// <summary>
    /// Sends a group invitation from one user to another.
    /// </summary>
    /// <param name="sendInvitation">The details of the invitation being sent.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of sending the invitation.</returns>
    Task<Util_GenericResponse<bool>>
    SendInvitation
    (
        DTO_SendInvitationRequest sendInvitation
    );
    /// <summary>
    /// Accepts a received group invitation.
    /// </summary>
    /// <param name="accepInvitation">The details of the invitation being accepted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of accepting the invitation.</returns>
    Task<Util_GenericResponse<bool>>
    AcceptInvitation
    (
        DTO_InvitationRequestActions accepInvitation
    );
    /// <summary>
    /// Rejects a received group invitation.
    /// </summary>
    /// <param name="rejectInvitation">The details of the invitation being rejected.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of rejecting the invitation.</returns>
    Task<Util_GenericResponse<bool>>
    RejectInvitation
    (
        DTO_InvitationRequestActions rejectInvitation
    );
    /// <summary>
    /// Deletes a previously sent group invitation.
    /// </summary>
    /// <param name="deleteInvitation">The details of the invitation being deleted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a generic response indicating the success or failure of deleting the invitation.</returns>
    Task<Util_GenericResponse<bool>>
    DeleteSentInvitation
    (
        DTO_InvitationRequestActions deleteInvitation
    );
}