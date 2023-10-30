using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.MediatR.Actions.Commands.GroupManagment;
using SafeShare.MediatR.Actions.Queries.GroupManagment;

namespace SafeShare.API.Controllers
{
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
            return await _mediator.Send(new MediatR_CreateGroupCommand(ownerId, createGroup));
        }
    }
}
