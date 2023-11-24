/*
 * Handles the MediatR_LoginUserCommand.
 * This handler receives a login request and uses the provided IAUTH_Login service to authenticate the user.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.Authentication.Interfaces;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.MediatR.Actions.Commands.Authentication;

namespace SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;

/// <summary>
/// Handles the MediatR_LoginUserCommand.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediatR_LoginUserCommandHandler"/> class.
/// </remarks>
/// <param name="loginUser">The IAUTH_Login service used to authenticate users.</param>
public class MediatR_LoginUserCommandHandler
(
    IAUTH_Login loginUser
) : IRequestHandler<MediatR_LoginUserCommand, Util_GenericResponse<DTO_LoginResult>>
{
    /// <summary>
    /// Handles the login request and returns an appropriate ObjectResult.
    /// </summary>
    /// <param name="request">The login request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An ObjectResult that represents the result of the login operation.</returns>
    public async Task<Util_GenericResponse<DTO_LoginResult>>
    Handle
    (
        MediatR_LoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var loginResult = await loginUser.LoginUser(request.Login);

        return loginResult;
    }
}