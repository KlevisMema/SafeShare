using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Queries.GroupManagment;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.API.Controllers;

public class GroupManagmentController : BaseController
{
    private readonly IMediator _mediator;

    public GroupManagmentController
    (
        IMediator mediator
    )
    {
        _mediator = mediator;
    }

    [HttpGet("GroupTypes/{userId}")]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupsTypes>>>
    GetGroupsTypes
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetGroupsTypesQuery(userId));
    }

    [HttpGet("GetGroupDetails/{userId}/{groupId}")]
    public async Task<ActionResult<Util_GenericResponse<DTO_GroupsTypes>>>
    GetGroupDetails
    (
        Guid userId,
        Guid groupId
    )
    {
        return await _mediator.Send(new MediatR_GetGroupDetailsQuery(userId, groupId));
    }

    [HttpPost("CreateGroup/{ownerId}")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    CreateGroup
    (
        Guid ownerId,
        [FromForm] DTO_CreateGroup createGroup
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_CreateGroupCommand(ownerId, createGroup));
    }

    [HttpPut("EditGroup/{userId}/{groupId}")]
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

    [HttpDelete("DeleteGroup/{ownerId}/{groupId}")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteGroup
    (
        Guid ownerId,
        Guid groupId
    )
    {
        return await _mediator.Send(new MediatR_DeleteGroupCommand(ownerId, groupId));
    }

    [HttpGet("GetGroupsInvitations")]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_RecivedInvitations>>>>
    GetGroupsInvitations
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetGetGroupsInvitationsQuery(userId));
    }

    [HttpGet("GetSentGroupInvitations")]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_SentInvitations>>>>
    GetSentGroupInvitations
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetSentGroupInvitationsQuery(userId));
    }

    [HttpPost("SendInvitation")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    SendInvitation
    (
        DTO_SendInvitationRequest dTO_SendInvitation
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_SendInvitationCommand(dTO_SendInvitation));
    }

    [HttpPost("AcceptInvitation")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    AcceptInvitation
    (
        DTO_InvitationRequestActions acceptInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_AcceptInvitationRequestCommand(acceptInvitationRequest));
    }

    [HttpPost("RejectInvitation")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    RejectInvitation
    (
        DTO_InvitationRequestActions rejectInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_RejectInvitationRequestCommand(rejectInvitationRequest));
    }

    [HttpDelete("DeleteInvitation")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteInvitation
    (
        DTO_InvitationRequestActions deleteInvitationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_DeleteSentInvitationCommand(deleteInvitationRequest));
    }

}