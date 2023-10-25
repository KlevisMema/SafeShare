using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Authorization;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.MediatR.Actions.Authentication.Commands;

namespace SafeShare.API.Controllers;

/// <summary>
///     A authentication contoller providing endpoinnts for login, logout and registering.
/// </summary>
public class AuthenticationController : BaseController
{
    private readonly IMediator _mediator;
    public AuthenticationController
    (
        IMediator mediator
    )
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     [POST] - 
    ///     Register user endpoint.
    ///     This endpoint is accessed by everyone by marking it with : <see cref="AllowAnonymousAttribute"/>.
    /// </summary>
    /// <param name="register"> 
    ///     Register data object value of type <see cref="DTO_Register"/>.
    /// </param>
    /// <returns> <see cref="Task"/> of <see cref="Util_GenericResponse{T}"/> where T is  <see cref="bool"/> </returns>
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

}