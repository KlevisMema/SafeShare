/*
 * Defines the authentication controller for the SafeShare API.
 * This controller provides endpoints for user authentication processes including registration, login, and token management.
*/

using MediatR;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using System.IdentityModel.Tokens.Jwt;
using SafeShare.Security.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SafeShare.ClientServerShared.Routes;
using SafeShare.Security.API.ActionFilters;
using SafeShare.DataTransormObject.Security;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.API.Controllers;

/// <summary>
/// Controller for handling authentication processes in the SafeShare application.
/// Provides endpoints for user registration, login, confirmation, and token management functionalities.
/// Uses MediatR for command and query handling, facilitating a CQRS pattern.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationController"/> class with the specified mediator.
/// </remarks>
/// <param name="mediator">The mediator used for command and query handling.</param>
[ApiController]
[Route(BaseRoute.Route)]
//[ServiceFilter(typeof(IApiKeyAuthorizationFilter))]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Endpoint to register a new user in the SafeShare system.
    /// Accepts user registration data and initiates the registration process.
    /// </summary>
    /// <param name="register">The registration data including user details.</param>
    /// <returns>A response indicating the success or failure of the registration process.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.Register)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    Register
    (
        [FromForm] DTO_Register register
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await mediator.Send(new MediatR_RegisterUserCommand(register));
    }
    /// <summary>
    /// Endpoint to confirm the registration of a new user.
    /// This typically involves verifying a token or a code sent to the user's email or phone.
    /// </summary>
    /// <param name="confirmRegistrationDto">The data required to confirm a user's registration.</param>
    /// <returns>A response indicating the success or failure of the registration confirmation process.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.ConfirmRegistration)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ConfirmRegistration
    (
        DTO_ConfirmRegistration confirmRegistrationDto
    )
    {
        return await mediator.Send(new MediatR_ConfirmUserRegistrationCommand(confirmRegistrationDto));
    }
    /// <summary>
    /// Endpoint for user login.
    /// Validates user credentials and generates an authentication token upon successful login.
    /// </summary>
    /// <param name="loginDto">The user's login credentials.</param>
    /// <returns>A response containing the authentication token or an error message.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.Login)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_LoginResult>>>
    LoginUser
    (
        [FromForm] DTO_Login loginDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await mediator.Send(new MediatR_LoginUserCommand(loginDto));
    }
    /// <summary>
    /// Endpoint to confirm a user's login, typically used in two-factor authentication processes.
    /// Validates the OTP (One-Time Password) sent to the user after the initial login attempt.
    /// </summary>
    /// <param name="userId">The identifier of the user attempting to confirm login.</param>
    /// <param name="otp">The one-time password for login confirmation.</param>
    /// <returns>A response containing the authentication token or an error message.</returns>
    [ServiceFilter(typeof(VerifyUser))]
    [HttpPost(Route_AuthenticationRoute.ConfirmLogin)]
    [Authorize(AuthenticationSchemes = "ConfirmLogin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_LoginResult>>>
    ConfirmLogin
    (
        Guid userId,
        string otp
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await mediator.Send(new MediatR_ConfirmLoginUserCommand(otp, userId));
    }
    /// <summary>
    /// Endpoint for requesting reconfirmation of the registration process.
    /// This can be used if the initial confirmation (like email verification) was not completed or needs to be resent.
    /// </summary>
    /// <param name="email">The email address of the user requesting reconfirmation.</param>
    /// <returns>A response indicating the success or failure of the registration reconfirmation request.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.ReConfirmRegistrationRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ReConfirmRegistrationRequest
    (
        string email
    )
    {
        return await mediator.Send(new MediatR_ReConfirmRegistrationRequestCommand(email));
    }
    /// <summary>
    /// Endpoint for logging out a user.
    /// Performs actions like invalidating the user's token to ensure proper logout.
    /// </summary>
    /// <param name="userId">The identifier of the user who is logging out.</param>
    /// <returns>A response indicating the success or failure of the logout process.</returns>
    [ServiceFilter(typeof(VerifyUser))]
    [HttpPost(Route_AuthenticationRoute.LogOut)]
    [Authorize(AuthenticationSchemes = "Default")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    public async Task<ActionResult>
    LogOut
    (
        Guid userId
    )
    {
        await mediator.Send(new MediatR_LogOutCommand(userId.ToString()));

        return Ok();
    }
    /// <summary>
    /// Endpoint for refreshing an expired authentication token.
    /// Validates the old token and issues a new one if the user is still authenticated.
    /// </summary>
    /// <param name="validateToken">The data required to validate and refresh the token.</param>
    /// <returns>A response indicating the success or failure of the token refresh process and includes the new token if successful.</returns>
    [AllowAnonymous]
    [HttpPost(Route_AuthenticationRoute.RefreshToken)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResult))]
    public async Task<ActionResult<Util_GenericResponse<DTO_Token>>>
    RefreshToken
    (
        DTO_ValidateToken validateToken
    )
    {
        return await mediator.Send(new MediatR_RefreshTokenCommand(validateToken));
    }
}