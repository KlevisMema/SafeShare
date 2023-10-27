using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Responses;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.MediatR.Actions.Queries.UserManagment;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.API.Controllers;

public class UserManagmentController : BaseController
{
    private readonly IMediator _mediator;

    public UserManagmentController
    (
        IMediator mediator
    )
    {
        _mediator = mediator;
    }

    [HttpGet("GetUser/{id}")]
    public async Task<ActionResult<Util_GenericResponse<DTO_UserUpdatedInfo>>>
    GetUser
    (
        Guid id
    )
    {
        return await _mediator.Send(new MediatR_GetUserQuery(id));
    }

    [HttpPost("UpdateUser/{id}")]
    public async Task<ActionResult<Util_GenericResponse<DTO_UserUpdatedInfo>>>
    UpdateUser
    (
        Guid id,
        [FromForm] DTO_UserInfo userInfo
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_UpdateUserCommand(id, userInfo));
    }

    [HttpPost("DeleteUser/{id}")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteUser
    (
        Guid id
    )
    {
        return await _mediator.Send(new MediatR_DeleteUserCommand(id));
    }

    [HttpPost("ChangePassword/{id}")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ChangePassword
    (
        Guid id,
        [FromForm] DTO_UserChangePassword changePassword
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ChangeUserPasswordCommand(id, changePassword));
    }
}