/*
 * Handles the MediatR_RegisterUserCommand.
 * This handler receives a registration request and uses the provided IAUTH_Register service to register the user.
*/

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.Utilities.Responses;
using SafeShare.Authentication.Interfaces;
using SafeShare.MediatR.Actions.Authentication.Commands;

namespace SafeShare.MediatR.Handlers.Authentication.CommandsHandler;

/// <summary>
/// Handles the MediatR_RegisterUserCommand.
/// </summary>
public class MediatR_RegisterUserCommandHandler : IRequestHandler<MediatR_RegisterUserCommand, ObjectResult>
{
    /// <summary>
    /// Gets the IAUTH_Register service used to register users.
    /// </summary>
    private readonly IAUTH_Register _register;
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatR_RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="registerUser">The IAUTH_Register service used to register users.</param>
    public MediatR_RegisterUserCommandHandler
    (
        IAUTH_Register registerUser
    )
    {
        _register = registerUser;
    }
    /// <summary>
    /// Handles the registration request and returns an appropriate ObjectResult.
    /// </summary>
    /// <param name="request">The registration request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An ObjectResult that represents the result of the registration operation.</returns>
    public async Task<ObjectResult>
    Handle
    (
        MediatR_RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var registerResult = await _register.RegisterUser(request.Register);

        return Util_GenericControllerResponse<bool>.ControllerResponse(registerResult);
    }
}