/*
 * Defines the authentication controller for the SafeShare API.
 * This controller provides endpoints for user authentication processes including registration and login.
*/

using MediatR;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.MediatR.Actions.Commands.Authentication;
using SafeShare.DataTransormObject.Security;

namespace SafeShare.API.Controllers;

/// <summary>
///     A authentication contoller providing endpoinnts for login and registering.
/// </summary>
public class AuthenticationController : BaseController
{
    /// <summary>
    /// The mediator used for command and query handling in the CQRS pattern.
    /// </summary>
    private readonly IMediator _mediator;
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationController"/> class with the specified mediator.
    /// </summary>
    /// <param name="mediator">The mediator used for command and query handling.</param>
    public AuthenticationController
    (
        IMediator mediator
    )
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="register">The registration data.</param>
    /// <returns>A response indicating the success or failure of the registration.</returns>
    [AllowAnonymous]
    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<bool>))]
    public async Task<ActionResult<Util_GenericResponse<bool>>> Register
    (
        [FromForm] DTO_Register register
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_RegisterUserCommand(register));
    }
    /// <summary>
    /// Confirms the registration of the user 
    /// </summary>
    /// <param name="confirmRegistrationDto">The <see cref="DTO_ConfirmRegistration"/> object </param>
    /// <returns></returns>
    [HttpPost("ConfirmRegistration")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ConfirmRegistration
    (
        DTO_ConfirmRegistration confirmRegistrationDto
    )
    {
        return await _mediator.Send(new MediatR_ConfirmUserRegistrationCommand(confirmRegistrationDto));
    }
    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="loginDto">The login data.</param>
    /// <returns>A response containing the token or an error message.</returns>
    [HttpPost("Login")]
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

        return await _mediator.Send(new MediatR_LoginUserCommand(loginDto));
    }
    /// <summary>
    /// Confirms the log in.
    /// </summary>
    /// <param name="otp">The otp </param>
    /// <returns>A response containing the token or an error message.</returns>
    [HttpPost("ConfirmLogin")]
    [Authorize(AuthenticationSchemes = "ConfirmLogin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<DTO_LoginResult>))]
    public async Task<ActionResult<Util_GenericResponse<DTO_LoginResult>>>
    ConfirmLogin
    (
        string otp
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return await _mediator.Send(new MediatR_ConfirmLoginUserCommand(otp));
    }
    /// <summary>
    /// Re confirms the registration proccess
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("ReConfirmRegistrationRequest")]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    ReConfirmRegistrationRequest
    (
        string email
    )
    {
        return await _mediator.Send(new MediatR_ReConfirmRegistrationRequestCommand(email));
    }
    /// <summary>
    /// Logs out a user
    /// </summary>
    /// <returns></returns>
    [HttpPost("LogOut")]
    [Authorize(AuthenticationSchemes = "Default")]
    public async Task<ActionResult>
    LogOut
    (
        Guid userId
    )
    {
        return await _mediator.Send(new MediatR_LogOutCommand(userId.ToString()));
    }

    [HttpPost("ValidateToken")]
    public async Task<ActionResult<Util_GenericResponse<DTO_Token>>>
    RefreshToken
    (
        DTO_ValidateToken validateToken
    )
    {
        return await _mediator.Send(new MediatR_RefreshTokenCommand(validateToken));
    }
}