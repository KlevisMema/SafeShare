/*
 * User Management Controller
 * 
 * This controller is responsible for all operations related to user management within the SafeShare application.
 * It provides endpoints to fetch user details, update user information, delete users, and change user passwords.
 * 
 * The controller leverages the MediatR library to dispatch requests to the appropriate handlers, 
 * ensuring a clean separation of concerns and promoting a CQRS pattern.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Authorization;
using SafeShare.DataTransormObject.UserManagment;
using SafeShare.MediatR.Actions.Queries.UserManagment;
using SafeShare.MediatR.Actions.Commands.UserManagment;

namespace SafeShare.API.Controllers;

/// <summary>
/// Controller responsible for managing users, including fetching, updating, deleting, and changing passwords.
/// </summary>
public class AccountManagmentController : BaseController
{
    /// <summary>
    /// Mediator pattern handler for dispatching requests.
    /// </summary>
    private readonly IMediator _mediator;
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountManagmentController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator pattern handler.</param>
    public AccountManagmentController
    (
        IMediator mediator
    )
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Fetch a user's updated information by their ID.
    /// </summary>
    /// <param name="id">Unique identifier of the user.</param>
    /// <returns>Returns user's updated information.</returns>
    [HttpGet("GetUser/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_UserUpdatedInfo>>>
    GetUser
    (
        Guid id
    )
    {
        return await _mediator.Send(new MediatR_GetUserQuery(id));
    }
    /// <summary>
    /// Update a user's information.
    /// </summary>
    /// <param name="id">Unique identifier of the user to be updated.</param>
    /// <param name="userInfo">User's new information.</param>
    /// <returns>Returns user's updated information.</returns>
    [HttpPut("UpdateUser/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
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
    /// <summary>
    /// Change a user's password by their ID.
    /// </summary>
    /// <param name="id">Unique identifier of the user.</param>
    /// <param name="changePassword">Details for changing the user's password.</param>
    /// <returns>Returns a boolean indicating the success of the password change operation.</returns>
    [HttpPut("ChangePassword/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
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
    /// <summary>
    /// Deactivate a user by their ID.
    /// </summary>
    /// <param name="id">Unique identifier of the user to be deactivated</param>
    /// <param name="deactivateAccount"> A dto containing user's information for deactivation process </param>
    /// <returns>Returns a boolean indicating the success of the deactivation operation.</returns>
    [HttpPost("DeactivateAccount/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeactivateAccount
    (
        Guid id,
        [FromForm] DTO_DeactivateAccount deactivateAccount
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_DeactivateAccountCommand(id, deactivateAccount));
    }

    [HttpPost("ActivateAccountRequest")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ActivateAccountRequest
    (
        string email
    )
    {
        return await _mediator.Send(new MediatR_ActivateAccountRequestCommand(email));
    }

    [HttpPost("ActivateAccountRequestConfirmation")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ActivateAccountRequestConfirmation
    (
        DTO_ActivateAccountConfirmation activateAccountConfirmationDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ActivateAccountConfirmationRequestCommand(activateAccountConfirmationDto));
    }

    [HttpPost("ForgotPassword")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ForgotPassword
    (
       [FromForm] string email
    )
    {
        return await _mediator.Send(new MediatR_ForgotPasswordCommand(email));
    }

    [HttpPost("ResetPassword")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ResetPassword
    (
        [FromForm] DTO_ResetPassword resetPassword
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ResetUserPasswordCommand(resetPassword));
    }

    [HttpPost("RequestChangeEmail")]
    [Authorize(AuthenticationSchemes = "Default")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    RequestChangeEmail
    (
        [FromForm] DTO_ChangeEmailAddressRequest emailAddress
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ChangeEmailAddressRequestCommand(emailAddress));
    }

    [Authorize(AuthenticationSchemes = "Default")]
    [HttpPost("ConfirmChangeEmailRequest")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ConfirmChangeEmailRequest
    (
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirmDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ChangeEmailAddressRequestConfirmationCommand(changeEmailAddressConfirmDto));
    }
}