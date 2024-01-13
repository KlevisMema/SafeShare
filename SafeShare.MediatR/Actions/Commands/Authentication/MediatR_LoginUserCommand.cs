/*
 * Represents a MediatR command for logging in a user.
 * This command is expected to return an ObjectResult upon execution.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;
using SafeShare.Utilities.SafeShareApi.Responses;

namespace SafeShare.MediatR.Actions.Commands.Authentication;

/// <summary>
/// Represents a MediatR command for logging in a user.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_LoginUserCommand"/> class.
/// </remarks>
/// <param name="login">The login details for the user.</param>
public class MediatR_LoginUserCommand
(
    DTO_Login login
) : /*IRequest<ObjectResult>*/ IRequest<Util_GenericResponse<DTO_LoginResult>>
{
    /// <summary>
    /// Gets or sets the login details for the user.
    /// </summary>
    public DTO_Login Login { get; set; } = login;
}