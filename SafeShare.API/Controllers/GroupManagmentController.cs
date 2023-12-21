/*
 * GroupManagementController class for SafeShare application.
 * This controller handles group management functionalities including creating, editing, 
 * deleting groups, managing group invitations, and retrieving group details.
 * Utilizes the MediatR library for CQRS pattern implementation.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SafeShare.ClientServerShared.Routes;
using SafeShare.Security.API.ActionFilters;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.MediatR.Actions.Queries.GroupManagment;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.API.Controllers;

/// <summary>
/// Controller for managing groups in the SafeShare application.
/// This controller includes functionalities for group creation, modification, deletion, 
/// and retrieval of group details and invitations.
/// It communicates with the business logic layer using MediatR for a clean, decoupled architecture.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="GroupManagmentController"/>
/// </remarks>
/// <param name="mediator">The instance of mediator used to send commands and queries</param>
//[ServiceFilter(typeof(VerifyUser))]
public class GroupManagmentController(IMediator mediator) : BaseController(mediator)
{
    /// <summary>
    /// Get group types of a user
    /// </summary>
    /// <param name="userId">The id of the user whose group types are to be retrieved</param>
    /// <returns>A task that represents the asynchronous operation, containing user's groups that he created and joined</returns>
    [HttpGet(Route_GroupManagmentRoutes.GroupTypes)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_GroupsTypes>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_GroupsTypes>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_GroupsTypes>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupsTypes>>>
    GetGroupsTypes
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetGroupsTypesQuery(userId));
    }
    /// <summary>
    /// Retrieves details of a specific group.
    /// </summary>
    /// <param name="userId">The identifier of the user requesting group details.</param>
    /// <param name="groupId">The identifier of the group whose details are being requested.</param>
    /// <returns>Detailed information about the specified group.</returns>
    [HttpGet(Route_GroupManagmentRoutes.GetGroupDetails)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_GroupDetails>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_GroupDetails>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_GroupDetails>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupDetails>>>
    GetGroupDetails
    (
        Guid userId,
        Guid groupId
    )
    {
        return await _mediator.Send(new MediatR_GetGroupDetailsQuery(userId, groupId));
    }
    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="userId">The identifier of the user creating the group.</param>
    /// <param name="createGroup">The details of the group to be created.</param>
    /// <returns>A boolean value indicating whether the group was successfully created.</returns>
    [HttpPost(Route_GroupManagmentRoutes.CreateGroup)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    CreateGroup
    (
        Guid userId,
        [FromForm] DTO_CreateGroup createGroup
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_CreateGroupCommand(userId, createGroup));
    }
    /// <summary>
    /// Edits the details of an existing group.
    /// </summary>
    /// <param name="userId">The identifier of the user editing the group.</param>
    /// <param name="groupId">The identifier of the group to be edited.</param>
    /// <param name="editGroup">The new details to update the group with.</param>
    /// <returns>A boolean value indicating whether the group was successfully edited.</returns>
    [HttpPut(Route_GroupManagmentRoutes.EditGroup)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_GroupType>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_GroupType>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_GroupType>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupType>>>
    EditGroup
    (
        Guid userId,
        Guid groupId,
        [FromForm] DTO_EditGroup editGroup
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_EditGroupCommand(userId, groupId, editGroup));
    }
    /// <summary>
    /// Deletes an existing group.
    /// </summary>
    /// <param name="userId">The identifier of the user deleting the group.</param>
    /// <param name="groupId">The identifier of the group to be deleted.</param>
    /// <returns>A boolean value indicating whether the group was successfully deleted.</returns>
    [HttpDelete(Route_GroupManagmentRoutes.DeleteGroup)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteGroup
    (
        Guid userId,
        Guid groupId
    )
    {
        return await _mediator.Send(new MediatR_DeleteGroupCommand(userId, groupId));
    }
    /// <summary>
    /// Retrieves received group invitations for a user.
    /// </summary>
    /// <param name="userId">The identifier of the user whose invitations are to be retrieved.</param>
    /// <returns>A list of received group invitations.</returns>
    [HttpGet(Route_GroupManagmentRoutes.GetGroupsInvitations)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<List<DTO_RecivedInvitations>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<List<DTO_RecivedInvitations>>))]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_RecivedInvitations>>>>
    GetGroupsInvitations
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetGetGroupsInvitationsQuery(userId));
    }
    /// <summary>
    /// Retrieves sent group invitations by a user.
    /// </summary>
    /// <param name="userId">The identifier of the user whose sent invitations are to be retrieved.</param>
    /// <returns>A list of sent group invitations.</returns>
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<List<DTO_SentInvitations>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<List<DTO_SentInvitations>>))]
    [HttpGet(Route_GroupManagmentRoutes.GetSentGroupInvitations)]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_SentInvitations>>>>
    GetSentGroupInvitations
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetSentGroupInvitationsQuery(userId));
    }
    /// <summary>
    /// Sends a group invitation.
    /// </summary>
    /// <param name="userId">The identifier of the user sending the invitation.</param>
    /// <param name="dTO_SendInvitation">The details of the invitation to be sent.</param>
    /// <returns>A boolean value indicating whether the invitation was successfully sent.</returns>
    [HttpPost(Route_GroupManagmentRoutes.SendInvitation)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    SendInvitation
    (
        Guid userId,
        DTO_SendInvitationRequest dTO_SendInvitation
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_SendInvitationCommand(dTO_SendInvitation));
    }
    /// <summary>
    /// Accepts a group invitation.
    /// </summary>
    /// <param name="userId">The identifier of the user accepting the invitation.</param>
    /// <param name="acceptInvitationRequest">The details of the invitation to be accepted.</param>
    /// <returns>A boolean value indicating whether the invitation was successfully accepted.</returns>
    [HttpPost(Route_GroupManagmentRoutes.AcceptInvitation)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    AcceptInvitation
    (
        Guid userId,
        DTO_InvitationRequestActions acceptInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_AcceptInvitationRequestCommand(acceptInvitationRequest));
    }
    /// <summary>
    /// Rejects a group invitation.
    /// </summary>
    /// <param name="userId">The identifier of the user rejecting the invitation.</param>
    /// <param name="rejectInvitationRequest">The details of the invitation to be rejected.</param>
    /// <returns>A boolean value indicating whether the invitation was successfully rejected.</returns>
    [HttpPost(Route_GroupManagmentRoutes.RejectInvitation)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    RejectInvitation
    (
        Guid userId,
        DTO_InvitationRequestActions rejectInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_RejectInvitationRequestCommand(rejectInvitationRequest));
    }
    /// <summary>
    /// Deletes a sent group invitation.
    /// </summary>
    /// <param name="userId">The identifier of the user deleting the invitation.</param>
    /// <param name="deleteInvitationRequest">The details of the invitation to be deleted.</param>
    /// <returns>A boolean value indicating whether the invitation was successfully deleted.</returns>
    [HttpDelete(Route_GroupManagmentRoutes.DeleteInvitation)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteInvitation
    (
        Guid userId,
        DTO_InvitationRequestActions deleteInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_DeleteSentInvitationCommand(deleteInvitationRequest));
    }
}