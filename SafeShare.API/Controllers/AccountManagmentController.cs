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
using SafeShare.ClientServer.Routes;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Authorization;
using SafeShare.Security.API.ActionFilters;
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
    /// Initializes a new instance of the <see cref="AccountManagmentController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator pattern handler.</param>
    public AccountManagmentController
    (
        IMediator mediator
    )
    : base
    (
        mediator
    )
    { }
    /// <summary>
    /// Fetch a user's updated information by their ID.
    /// </summary>
    /// <param name="userId">Unique identifier of the user.</param>
    /// <returns>Returns user's updated information.</returns>
    [HttpGet(Route_AccountManagmentRoute.GetUser)]
    [ServiceFilter(typeof(VerifyUser))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_UserUpdatedInfo>>>
    GetUser
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_GetUserQuery(userId));
    }
    /// <summary>
    /// Update a user's information.
    /// </summary>
    /// <param name="userId">Unique identifier of the user to be updated.</param>
    /// <param name="userInfo">User's new information.</param>
    /// <returns>Returns user's updated information.</returns>
    [HttpPut(Route_AccountManagmentRoute.UpdateUser)]
    [ServiceFilter(typeof(VerifyUser))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_UserUpdatedInfo>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_UserUpdatedInfo>>>
    UpdateUser
    (
        Guid userId,
        [FromForm] DTO_UserInfo userInfo
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_UpdateUserCommand(userId, userInfo));
    }
    /// <summary>
    /// Change a user's password by their ID.
    /// </summary>
    /// <param name="userId">Unique identifier of the user.</param>
    /// <param name="changePassword">Details for changing the user's password.</param>
    /// <returns>Returns a boolean indicating the success of the password change operation.</returns>
    [ServiceFilter(typeof(VerifyUser))]
    [HttpPut(Route_AccountManagmentRoute.ChangePassword)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ChangePassword
    (
        Guid userId,
        [FromForm] DTO_UserChangePassword changePassword
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ChangeUserPasswordCommand(userId, changePassword));
    }
    /// <summary>
    /// Deactivate a user by their ID.
    /// </summary>
    /// <param name="userId">Unique identifier of the user to be deactivated</param>
    /// <param name="deactivateAccount"> A dto containing user's information for deactivation process </param>
    /// <returns>Returns a boolean indicating the success of the deactivation operation.</returns>
    [ServiceFilter(typeof(VerifyUser))]
    [HttpPost(Route_AccountManagmentRoute.DeactivateAccount)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeactivateAccount
    (
        Guid userId,
        [FromForm] DTO_DeactivateAccount deactivateAccount
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_DeactivateAccountCommand(userId, deactivateAccount));
    }
    /// <summary>
    /// Sends a request to activate an account based on the provided email address.
    /// </summary>
    /// <param name="email">The email address associated with the account to be activated.</param>
    /// <returns>A response indicating the success or failure of the account activation request.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ActivateAccountRequest)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ActivateAccountRequest
    (
        string email
    )
    {
        return await _mediator.Send(new MediatR_ActivateAccountRequestCommand(email));
    }
    /// <summary>
    /// Confirms the account activation request using a specific confirmation token or code.
    /// </summary>
    /// <param name="activateAccountConfirmationDto">The DTO containing the confirmation details for account activation.</param>
    /// <returns>A response indicating the success or failure of the account activation confirmation.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ActivateAccountRequestConfirmation)]
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
    /// <summary>
    /// Initiates the password reset process for a user based on their email address.
    /// </summary>
    /// <param name="email">The email address of the user who forgot their password.</param>
    /// <returns>A response indicating the success or failure of the forgot password request.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ForgotPassword)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ForgotPassword
    (
       [FromForm] string email
    )
    {
        return await _mediator.Send(new MediatR_ForgotPasswordCommand(email));
    }
    /// <summary>
    /// Allows a user to reset their password using a password reset token.
    /// </summary>
    /// <param name="resetPassword">The DTO containing the new password and reset token information.</param>
    /// <returns>A response indicating the success or failure of the password reset operation.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AccountManagmentRoute.ResetPassword)]
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
    /// <summary>
    /// Sends a request to change the email address associated with a user's account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user requesting the email change.</param>
    /// <param name="emailAddress">The DTO containing the new email address information.</param>
    /// <returns>A response indicating the success or failure of the email change request.</returns>
    [ServiceFilter(typeof(VerifyUser))]
    [HttpPost(Route_AccountManagmentRoute.RequestChangeEmail)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    RequestChangeEmail
    (
        Guid userId,
        [FromForm] DTO_ChangeEmailAddressRequest emailAddress
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ChangeEmailAddressRequestCommand(userId, emailAddress));
    }
    /// <summary>
    /// Confirms the email change request for a user's account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user confirming the email change.</param>
    /// <param name="changeEmailAddressConfirmDto">The DTO containing confirmation details for the email change.</param>
    /// <returns>A response indicating the success or failure of the email change confirmation.</returns>
    [ServiceFilter(typeof(VerifyUser))]
    [HttpPost(Route_AccountManagmentRoute.ConfirmChangeEmailRequest)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ConfirmChangeEmailRequest
    (
        Guid userId,
        DTO_ChangeEmailAddressRequestConfirm changeEmailAddressConfirmDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ChangeEmailAddressRequestConfirmationCommand(userId, changeEmailAddressConfirmDto));
    }
}