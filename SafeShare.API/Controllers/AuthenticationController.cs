/*
 * Defines the authentication controller for the SafeShare API.
 * This controller provides endpoints for user authentication processes including registration and login.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Authorization;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.MediatR.Actions.Commands.Authentication;

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

        var result = await _mediator.Send(new MediatR_RegisterUserCommand(register));

        return result;
    }
    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="loginDto">The login data.</param>
    /// <returns>A response containing the token or an error message.</returns>
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Util_GenericResponse<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Util_GenericResponse<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Util_GenericResponse<string>))]
    public async Task<ActionResult<Util_GenericResponse<string>>>
    LoginUser
    (
        [FromForm] DTO_Login loginDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _mediator.Send(new MediatR_LoginUserCommand(loginDto));

        return result;
    }
}